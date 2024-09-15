using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UniTool.Utilities.Editor
{
    //TODO: better handling for serializedObjects with mixed values
    //TODO: make it not rely on GUILayout at all, so its safe to use under PropertyDrawers.
    public class UniReorderableList
    {
        public delegate void HeaderCallbackDelegate(Rect rect);
        public delegate void FooterCallbackDelegate(Rect rect);
        public delegate void ElementCallbackDelegate(Rect rect, int index, bool isActive, bool isFocused);
        public delegate float ElementHeightCallbackDelegate(int index);
        public delegate void DrawNoneElementCallback(Rect rect);
        
        public delegate bool ReorderDecideCallbackDelegate(UniReorderableList list, int oldIndex, int newIndex);
        public delegate void ReorderCallbackDelegateWithDetails(UniReorderableList list, int oldIndex, int newIndex);
        public delegate void ReorderCallbackDelegate(UniReorderableList list);
        public delegate void SelectCallbackDelegate(UniReorderableList list);
        public delegate void AddCallbackDelegate(UniReorderableList list);
        public delegate void AddDropdownCallbackDelegate(Rect buttonRect, UniReorderableList list);
        public delegate void RemoveCallbackDelegate(UniReorderableList list);
        public delegate void ChangedCallbackDelegate(UniReorderableList list);
        public delegate bool CanRemoveCallbackDelegate(UniReorderableList list);
        public delegate bool CanAddCallbackDelegate(UniReorderableList list);
        public delegate void DragCallbackDelegate(UniReorderableList list);


        // draw callbacks
        public HeaderCallbackDelegate DrawHeaderCallback;
        public FooterCallbackDelegate DrawFooterCallback = null;
        public ElementCallbackDelegate DrawElementCallback;
        public ElementCallbackDelegate DrawElementBackgroundCallback;
        private DrawNoneElementCallback drawNoneElementCallback = null;

        // layout callbacks
        // if supplying own element heights, try to cache the results as this may be called frequently
        public ElementHeightCallbackDelegate ElementHeightCallback;

        // interaction callbacks
        public ReorderDecideCallbackDelegate OnReorderDecide = (_1, _2, _3) => true;
        public ReorderCallbackDelegateWithDetails OnReorderCallbackWithDetails;
        public ReorderCallbackDelegate OnReorderCallback;
        public SelectCallbackDelegate OnSelectCallback;
        public AddCallbackDelegate OnAddCallback;
        public AddDropdownCallbackDelegate OnAddDropdownCallback;
        public RemoveCallbackDelegate OnRemoveCallback;
        public DragCallbackDelegate OnMouseDragCallback;
        public SelectCallbackDelegate OnMouseUpCallback;
        public CanRemoveCallbackDelegate OnCanRemoveCallback;
        public CanAddCallbackDelegate OnCanAddCallback;
        public ChangedCallbackDelegate OnChangedCallback;

        private int m_ActiveElement = -1;
        private float m_DragOffset = 0;
        private object m_SlideGroup;

        private SerializedObject m_SerializedObject;
        private SerializedProperty m_Elements;
        private IList m_ElementList;
        private bool m_Draggable;
        private float m_DraggedY;
        private bool m_Dragging;
        private List<int> m_NonDragTargetIndices;

        private bool m_DisplayHeader;
        public bool DisplayAdd;
        public bool DisplayRemove;

        private int id = -1;

        // class for default rendering and behavior of reorderable list - stores styles and is statically available as s_Defaults
        public class Defaults
        {
            public GUIContent iconToolbarPlus = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add to list");
            public GUIContent iconToolbarPlusMore = EditorGUIUtility.TrIconContent("Toolbar Plus More", "Choose to add to list");
            public GUIContent iconToolbarMinus = EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection from list");
            public readonly GUIStyle draggingHandle = "RL DragHandle";
            public readonly GUIStyle headerBackground = "RL Header";
            private readonly GUIStyle emptyHeaderBackground = "RL Empty Header";
            public readonly GUIStyle footerBackground = "RL Footer";
            public readonly GUIStyle boxBackground = "RL Background";
            public readonly GUIStyle preButton = "RL FooterButton";
            public readonly GUIStyle elementBackground = "RL Element";
            public const int padding = 6;
            public const int dragHandleWidth = 20;
            private static GUIContent s_ListIsEmpty = EditorGUIUtility.TrTextContent("List is Empty");

            // draw the default footer
            public void DrawFooter(Rect rect, UniReorderableList list)
            {
                float rightEdge = rect.xMax - 10f;
                float leftEdge = rightEdge - 8f;
                if (list.DisplayAdd)
                    leftEdge -= 25;
                if (list.DisplayRemove)
                    leftEdge -= 25;
                rect = new Rect(leftEdge, rect.y, rightEdge - leftEdge, rect.height);
                Rect addRect = new Rect(leftEdge + 4, rect.y, 25, 16);
                Rect removeRect = new Rect(rightEdge - 29, rect.y, 25, 16);
                if (Event.current.type == EventType.Repaint)
                {
                    footerBackground.Draw(rect, false, false, false, false);
                }
                if (list.DisplayAdd)
                {
                    using (new EditorGUI.DisabledScope(
                        list.OnCanAddCallback != null && !list.OnCanAddCallback(list)))
                    {
                        if (GUI.Button(addRect, list.OnAddDropdownCallback != null ? iconToolbarPlusMore : iconToolbarPlus, preButton))
                        {
                            if (list.OnAddDropdownCallback != null)
                                list.OnAddDropdownCallback(addRect, list);
                            else if (list.OnAddCallback != null)
                                list.OnAddCallback(list);
                            else
                                DoAddButton(list);

                            if (list.OnChangedCallback != null)
                                list.OnChangedCallback(list);
                        }
                    }
                }
                if (list.DisplayRemove)
                {
                    using (new EditorGUI.DisabledScope(
                        list.Index < 0 || list.Index >= list.count ||
                        (list.OnCanRemoveCallback != null && !list.OnCanRemoveCallback(list))))
                    {
                        if (GUI.Button(removeRect, iconToolbarMinus, preButton))
                        {
                            if (list.OnRemoveCallback == null)
                                DoRemoveButton(list);
                            else
                                list.OnRemoveCallback(list);

                            if (list.OnChangedCallback != null)
                                list.OnChangedCallback(list);
                        }
                    }
                }
            }

            // default add button behavior
            public void DoAddButton(UniReorderableList list)
            {
                if (list.SerializedProperty != null)
                {
                    list.SerializedProperty.arraySize += 1;
                    list.Index = list.SerializedProperty.arraySize - 1;
                }
                else
                {
                    // this is ugly but there are a lot of cases like null types and default constructors
                    Type elementType = list.List.GetType().GetElementType();
                    if (elementType == typeof(string))
                        list.Index = list.List.Add("");
                    else if (elementType != null && elementType.GetConstructor(Type.EmptyTypes) == null)
                        Debug.LogError("Cannot add element. Type " + elementType.ToString() + " has no default constructor. Implement a default constructor or implement your own add behaviour.");
                    else if (list.List.GetType().GetGenericArguments()[0] != null)
                        list.Index = list.List.Add(Activator.CreateInstance(list.List.GetType().GetGenericArguments()[0]));
                    else if (elementType != null)
                        list.Index = list.List.Add(Activator.CreateInstance(elementType));
                    else
                        Debug.LogError("Cannot add element of type Null.");
                }
            }

            // default remove button behavior
            public void DoRemoveButton(UniReorderableList list)
            {
                if (list.SerializedProperty != null)
                {
                    list.SerializedProperty.DeleteArrayElementAtIndex(list.Index);
                    if (list.Index >= list.SerializedProperty.arraySize - 1)
                        list.Index = list.SerializedProperty.arraySize - 1;
                }
                else
                {
                    list.List.RemoveAt(list.Index);
                    if (list.Index >= list.List.Count - 1)
                        list.Index = list.List.Count - 1;
                }
            }

            // draw the default header background
            public void DrawHeaderBackground(Rect headerRect)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    // We assume that a height smaller than 5px means a header with no content
                    if (headerRect.height < 5f)
                    {
                        emptyHeaderBackground.Draw(headerRect, false, false, false, false);
                    }
                    else
                    {
                        headerBackground.Draw(headerRect, false, false, false, false);
                    }
                }
            }

            // draw the default header
            public void DrawHeader(Rect headerRect, SerializedObject serializedObject, SerializedProperty element, IList elementList)
            {
                EditorGUI.LabelField(headerRect, EditorGUIHelper.TempContent((element != null) ? "Serialized Property" : "IList"));
            }

            // draw the default element background
            public void DrawElementBackground(Rect rect, int index, bool selected, bool focused, bool draggable)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    elementBackground.Draw(rect, false, selected, selected, focused);
                }
            }

            public void DrawElementDraggingHandle(Rect rect, int index, bool selected, bool focused, bool draggable)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    if (draggable)
                        draggingHandle.Draw(new Rect(rect.x + 5, rect.y + 8, 10, 6), false, false, false, false);
                }
            }

            // draw the default element
            public void DrawElement(Rect rect, SerializedProperty element, System.Object listItem, bool selected, bool focused, bool draggable)
            {
                EditorGUI.LabelField(rect, EditorGUIHelper.TempContent((element != null) ? element.displayName : listItem.ToString()));
            }

            // draw the default element
            public void DrawNoneElement(Rect rect, bool draggable)
            {
                EditorGUI.LabelField(rect, Defaults.s_ListIsEmpty);
            }
        }
        static Defaults s_Defaults;
        public static Defaults defaultBehaviours
        {
            get
            {
                return s_Defaults;
            }
        }

        // constructors
        public UniReorderableList(IList elements)
        {
            InitList(null, null, elements, true, true, true, true);
        }

        public UniReorderableList(IList elements, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
        {
            InitList(null, null, elements, draggable, displayHeader, displayAddButton, displayRemoveButton);
        }

        public UniReorderableList(SerializedObject serializedObject, SerializedProperty elements)
        {
            InitList(serializedObject, elements, null, true, true, true, true);
        }

        public UniReorderableList(SerializedObject serializedObject, SerializedProperty elements, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
        {
            InitList(serializedObject, elements, null, draggable, displayHeader, displayAddButton, displayRemoveButton);
        }

        private void InitList(SerializedObject serializedObject, SerializedProperty elements, IList elementList, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
        {
            id = GUIUtility.GetControlID(FocusType.Passive);
            m_SerializedObject = serializedObject;
            m_Elements = elements;
            m_ElementList = elementList;
            m_Draggable = draggable;
            m_Dragging = false;
            m_SlideGroup = Type.GetType("UnityEditor.GUISlideGroup, UnityEditor").CreateInstance();
            DisplayAdd = displayAddButton;
            m_DisplayHeader = displayHeader;
            DisplayRemove = displayRemoveButton;
            if (m_Elements != null && m_Elements.editable == false)
                m_Draggable = false;
            if (m_Elements != null && m_Elements.isArray == false)
                Debug.LogError("Input elements should be an Array SerializedProperty");
        }

        public SerializedProperty SerializedProperty
        {
            get { return m_Elements; }
            set { m_Elements = value; }
        }

        public IList List
        {
            get { return m_ElementList; }
            set { m_ElementList = value; }
        }


        // active element index accessor
        public int Index
        {
            get { return m_ActiveElement; }
            set { m_ActiveElement = value; }
        }

        // individual element height accessor
        public float ElementHeight = 21;
        // header height accessor
        public float HeaderHeight = 20;
        // footer height accessor
        public float FooterHeight = 20;
        // show default background
        public bool ShowDefaultBackground = true;
        private float listElementTopPadding => HasListElementTopPadding ? (HeaderHeight > 5 ? 4 : 1) : 0; // headerHeight is usually set to 3px when there is no header content. Therefore, we add a 1px top margin to match the 4px bottom margin
        private const float kListElementBottomPadding = 4;

        public bool HasListElementTopPadding = true;

        // draggable accessor
        public bool Draggable
        {
            get { return m_Draggable; }
            set { m_Draggable = value; }
        }

        private Rect GetContentRect(Rect rect)
        {
            Rect r = rect;

            if (Draggable)
                r.xMin += Defaults.dragHandleWidth;
            else
                r.xMin += Defaults.padding;
            r.xMax -= Defaults.padding;
            return r;
        }

        private float GetElementYOffset(int index)
        {
            return GetElementYOffset(index, -1);
        }

        private float GetElementYOffset(int index, int skipIndex)
        {
            if (ElementHeightCallback == null)
                return index * ElementHeight;

            float offset = 0;
            for (int i = 0; i < index; i++)
            {
                if (i != skipIndex)
                    offset += ElementHeightCallback(i);
            }
            return offset;
        }

        private float GetElementHeight(int index)
        {
            if (ElementHeightCallback == null)
                return ElementHeight;

            return ElementHeightCallback(index);
        }

        private Rect GetRowRect(int index, Rect listRect)
        {
            return new Rect(listRect.x, listRect.y + GetElementYOffset(index), listRect.width, GetElementHeight(index));
        }

        public int count
        {
            get
            {
                if (m_Elements != null)
                {
                    if (!m_Elements.hasMultipleDifferentValues)
                        return m_Elements.arraySize;

                    int smallerArraySize = m_Elements.arraySize;
                    foreach (var targetObject in m_Elements.serializedObject.targetObjects)
                    {
                        SerializedObject serializedObject = new SerializedObject(targetObject);
                        SerializedProperty property = serializedObject.FindProperty(m_Elements.propertyPath);
                        smallerArraySize = Math.Min(property.arraySize, smallerArraySize);
                    }
                    return smallerArraySize;
                }
                return m_ElementList.Count;
            }
        }

        public void DoLayoutList(bool drawHeader = true, bool drawElements = true, bool drawFooter = true) //TODO: better API?
        {
            if (s_Defaults == null)
                s_Defaults = new Defaults();

            if (drawHeader)
            {
                // do the custom or default header GUI
                Rect headerRect = GUILayoutUtility.GetRect(0, HeaderHeight, GUILayout.ExpandWidth(true));
                // do the parts of our list
                DoListHeader(headerRect);
            }

            if (drawElements)
            {
                //Elements area
                Rect listRect = GUILayoutUtility.GetRect(10, GetListElementHeight(), GUILayout.ExpandWidth(true));
                DoListElements(listRect);
            }

            if (drawFooter)
            {
                // do the footer GUI
                Rect footerRect = GUILayoutUtility.GetRect(4, FooterHeight, GUILayout.ExpandWidth(true));
                DoListFooter(footerRect);
            }
        }

        public void DoList(Rect rect) //TODO: better API?
        {
            if (s_Defaults == null)
                s_Defaults = new Defaults();

            // do the custom or default header GUI
            Rect headerRect = new Rect(rect.x, rect.y, rect.width, HeaderHeight);
            //Elements area
            Rect listRect = new Rect(rect.x, headerRect.y + headerRect.height, rect.width, GetListElementHeight());
            // do the footer GUI
            Rect footerRect = new Rect(rect.x, listRect.y + listRect.height, rect.width, FooterHeight);

            // do the parts of our list
            DoListHeader(headerRect);
            DoListElements(listRect);
            DoListFooter(footerRect);
        }

        public float GetHeight()
        {
            float totalheight = 0f;
            totalheight += GetListElementHeight();
            totalheight += HeaderHeight;
            totalheight += FooterHeight;
            return totalheight;
        }

        private float GetListElementHeight()
        {
            float listElementPadding = kListElementBottomPadding + listElementTopPadding;

            int arraySize = count;
            if (arraySize == 0)
            {
                return ElementHeight + listElementPadding;
            }

            if (ElementHeightCallback != null)
            {
                return GetElementYOffset(arraySize - 1) + GetElementHeight(arraySize - 1) + listElementPadding;
            }

            return (ElementHeight * arraySize) + listElementPadding;
        }

        private void DoListElements(Rect listRect)
        {
            // How many elements? If none, make space for showing default line that shows no elements are present
            int arraySize = count;

            // draw the background in repaint
            if (ShowDefaultBackground && Event.current.type == EventType.Repaint)
                s_Defaults.boxBackground.Draw(listRect, false, false, false, false);

            // resize to the area that we want to draw our elements into
            listRect.yMin += listElementTopPadding; listRect.yMax -= kListElementBottomPadding;

            if (ShowDefaultBackground)
            {
                listRect.xMin += 1;
                listRect.xMax -= 1;
            }

            // create the rect for individual elements in the list
            Rect elementRect = listRect;
            elementRect.height = ElementHeight;

            // the content rect is what we will actually draw into -- it doesn't include the drag handle or padding
            Rect elementContentRect = elementRect;


            if (((m_Elements != null && m_Elements.isArray == true) || m_ElementList != null) && arraySize > 0)
            {
                // If there are elements, we need to draw them -- we will do this differently depending on if we are dragging or not
                if (IsDragging() && Event.current.type == EventType.Repaint)
                {
                    // we are dragging, so we need to build the new list of target indices
                    int targetIndex = CalculateRowIndex();
                    m_NonDragTargetIndices.Clear();
                    for (int i = 0; i < arraySize; i++)
                    {
                        if (i != m_ActiveElement)
                            m_NonDragTargetIndices.Add(i);
                    }
                    m_NonDragTargetIndices.Insert(targetIndex, -1);

                    // now draw each element in the list (excluding the active element)
                    bool targetSeen = false;
                    for (int i = 0; i < m_NonDragTargetIndices.Count; i++)
                    {
                        if (m_NonDragTargetIndices[i] != -1)
                        {
                            elementRect.height = GetElementHeight(i);
                            // update the position of the rect (based on element position and accounting for sliding)
                            if (ElementHeightCallback == null)
                            {
                                elementRect.y = listRect.y + GetElementYOffset(i, m_ActiveElement);
                            }
                            else
                            {
                                elementRect.y = listRect.y + GetElementYOffset(m_NonDragTargetIndices[i], m_ActiveElement);
                                if (targetSeen)
                                {
                                    elementRect.y += ElementHeightCallback(m_ActiveElement);
                                }
                            }
                            elementRect = (Rect)m_SlideGroup.GetType().InvokeMethod("GetRect", m_SlideGroup, m_NonDragTargetIndices[i], elementRect);

                            // actually draw the element
                            if (DrawElementBackgroundCallback == null)
                                s_Defaults.DrawElementBackground(elementRect, i, false, false, m_Draggable);
                            else
                                DrawElementBackgroundCallback(elementRect, i, false, false);

                            s_Defaults.DrawElementDraggingHandle(elementRect, i, false, false, m_Draggable);

                            elementContentRect = GetContentRect(elementRect);
                            if (DrawElementCallback == null)
                            {
                                if (m_Elements != null)
                                    s_Defaults.DrawElement(elementContentRect, m_Elements.GetArrayElementAtIndex(m_NonDragTargetIndices[i]), null, false, false, m_Draggable);
                                else
                                    s_Defaults.DrawElement(elementContentRect, null, m_ElementList[m_NonDragTargetIndices[i]], false, false, m_Draggable);
                            }
                            else
                            {
                                DrawElementCallback(elementContentRect, m_NonDragTargetIndices[i], false, false);
                            }
                        }
                        else
                        {
                            targetSeen = true;
                        }
                    }

                    // finally get the position of the active element
                    elementRect.y = m_DraggedY - m_DragOffset + listRect.y;

                    // actually draw the element
                    if (DrawElementBackgroundCallback == null)
                        s_Defaults.DrawElementBackground(elementRect, m_ActiveElement, true, true, m_Draggable);
                    else
                        DrawElementBackgroundCallback(elementRect, m_ActiveElement, true, true);

                    s_Defaults.DrawElementDraggingHandle(elementRect, m_ActiveElement, true, true, m_Draggable);


                    elementContentRect = GetContentRect(elementRect);

                    // draw the active element
                    if (DrawElementCallback == null)
                    {
                        if (m_Elements != null)
                            s_Defaults.DrawElement(elementContentRect, m_Elements.GetArrayElementAtIndex(m_ActiveElement), null, true, true, m_Draggable);
                        else
                            s_Defaults.DrawElement(elementContentRect, null, m_ElementList[m_ActiveElement], true, true, m_Draggable);
                    }
                    else
                    {
                        DrawElementCallback(elementContentRect, m_ActiveElement, true, true);
                    }
                }
                else
                {
                    // if we aren't dragging, we just draw all of the elements in order
                    for (int i = 0; i < arraySize; i++)
                    {
                        bool activeElement = (i == m_ActiveElement);
                        bool focusedElement =  (i == m_ActiveElement && HasKeyboardControl());

                        // update the position of the element
                        elementRect.height = GetElementHeight(i);
                        elementRect.y = listRect.y + GetElementYOffset(i);

                        // draw the background
                        if (DrawElementBackgroundCallback == null)
                            s_Defaults.DrawElementBackground(elementRect, i, activeElement, focusedElement, m_Draggable);
                        else
                            DrawElementBackgroundCallback(elementRect, i, activeElement, focusedElement);
                        s_Defaults.DrawElementDraggingHandle(elementRect, i, activeElement, focusedElement, m_Draggable);


                        elementContentRect = GetContentRect(elementRect);

                        // do the callback for the element
                        if (DrawElementCallback == null)
                        {
                            if (m_Elements != null)
                                s_Defaults.DrawElement(elementContentRect, m_Elements.GetArrayElementAtIndex(i), null, activeElement, focusedElement, m_Draggable);
                            else
                                s_Defaults.DrawElement(elementContentRect, null, m_ElementList[i], activeElement, focusedElement, m_Draggable);
                        }
                        else
                        {
                            DrawElementCallback(elementContentRect, i, activeElement, focusedElement);
                        }
                    }
                }

                // handle the interaction
                DoDraggingAndSelection(listRect);
            }
            else
            {
                // there was no content, so we will draw an empty element
                elementRect.y = listRect.y;
                // draw the background
                if (DrawElementBackgroundCallback == null)
                    s_Defaults.DrawElementBackground(elementRect, -1, false, false, false);
                else
                    DrawElementBackgroundCallback(elementRect, -1, false, false);
                s_Defaults.DrawElementDraggingHandle(elementRect, -1, false, false, false);

                elementContentRect = elementRect;
                elementContentRect.xMin += Defaults.padding;
                elementContentRect.xMax -= Defaults.padding;
                if (drawNoneElementCallback == null)
                    s_Defaults.DrawNoneElement(elementContentRect, m_Draggable);
                else
                    drawNoneElementCallback(elementContentRect);
            }
        }

        private void DoListHeader(Rect headerRect)
        {
            // draw the background on repaint
            if (ShowDefaultBackground && Event.current.type == EventType.Repaint)
                s_Defaults.DrawHeaderBackground(headerRect);

            // apply the padding to get the internal rect
            headerRect.xMin += Defaults.padding;
            headerRect.xMax -= Defaults.padding;
            headerRect.height -= 2;
            headerRect.y += 1;

            // perform the default or overridden callback
            if (DrawHeaderCallback != null)
                DrawHeaderCallback(headerRect);
            else if (m_DisplayHeader)
                s_Defaults.DrawHeader(headerRect, m_SerializedObject, m_Elements, m_ElementList);
        }

        private void DoListFooter(Rect footerRect)
        {
            // perform callback or the default footer
            if (DrawFooterCallback != null)
                DrawFooterCallback(footerRect);
            else if (DisplayAdd || DisplayRemove)
                s_Defaults.DrawFooter(footerRect, this); // draw the footer if the add or remove buttons are required
        }

        private void DoDraggingAndSelection(Rect listRect)
        {
            Event evt = Event.current;
            int oldIndex = m_ActiveElement;
            bool clicked = false;
            switch (evt.GetTypeForControl(id))
            {
                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl != id)
                        return;
                    // if we have keyboard focus, arrow through the list
                    if (evt.keyCode == KeyCode.DownArrow)
                    {
                        m_ActiveElement += 1;
                        evt.Use();
                    }
                    if (evt.keyCode == KeyCode.UpArrow)
                    {
                        m_ActiveElement -= 1;
                        evt.Use();
                    }
                    if (evt.keyCode == KeyCode.Escape && GUIUtility.hotControl == id)
                    {
                        GUIUtility.hotControl = 0;
                        m_Dragging = false;
                        evt.Use();
                    }
                    // don't allow arrowing through the ends of the list
                    m_ActiveElement = Mathf.Clamp(m_ActiveElement, 0, (m_Elements != null) ? m_Elements.arraySize - 1 : m_ElementList.Count - 1);
                    break;

                case EventType.MouseDown:

                    if (!listRect.Contains(Event.current.mousePosition) || (Event.current.button != 0 && Event.current.button != 1))
                        break;

                    // clicking on the list should end editing any existing edits
                    EditorGUIHelper.EndEditingActiveTextField();
                    // pick the active element based on click position
                    m_ActiveElement = GetRowIndex(Event.current.mousePosition.y - listRect.y);

                    if (m_Draggable && Event.current.button == 0)
                    {
                        // if we can drag, set the hot control and start dragging (storing the offset)
                        m_DragOffset = (Event.current.mousePosition.y - listRect.y) - GetElementYOffset(m_ActiveElement);
                        UpdateDraggedY(listRect);
                        GUIUtility.hotControl = id;
                        m_SlideGroup.GetType().InvokeMethod("Reset", m_SlideGroup);
                        m_NonDragTargetIndices = new List<int>();
                    }
                    GrabKeyboardFocus();

                    // Prevent consuming the right mouse event in order to enable the context menu
                    if (Event.current.button == 1)
                        break;

                    evt.Use();
                    clicked = true;
                    break;

                case EventType.MouseDrag:
                    if (!m_Draggable || GUIUtility.hotControl != id)
                        break;

                    // Set m_Dragging state on first MouseDrag event after we got hotcontrol (to prevent animating elements when deleting elements by context menu)
                    m_Dragging = true;

                    if (OnMouseDragCallback != null)
                        OnMouseDragCallback(this);

                    // if we are dragging, update the position
                    UpdateDraggedY(listRect);
                    evt.Use();
                    break;

                case EventType.MouseUp:
                    if (!m_Draggable)
                    {
                        // if mouse up was on the same index as mouse down we fire a mouse up callback (useful if for beginning renaming on mouseup)
                        if (OnMouseUpCallback != null && IsMouseInsideActiveElement(listRect))
                        {
                            // set the keyboard control
                            OnMouseUpCallback(this);
                        }
                        break;
                    }

                    // hotcontrol is only set when list is draggable
                    if (GUIUtility.hotControl != id)
                        break;
                    evt.Use();
                    m_Dragging = false;

                    try
                    {
                        // What will be the index of this if we release?
                        int targetIndex = CalculateRowIndex();
                        if (m_ActiveElement != targetIndex)
                        {
                            // if the target index is different than the current index...
                            if (m_SerializedObject != null && m_Elements != null)
                            {
                                // if we are working with Serialized Properties, we can handle it for you
                                m_Elements.MoveArrayElement(m_ActiveElement, targetIndex);
                                m_SerializedObject.ApplyModifiedProperties();
                                m_SerializedObject.Update();
                            }
                            else if (m_ElementList != null)
                            {
                                // we are working with the IList, which is probably of a fixed length
                                if (OnReorderDecide?.Invoke(this, m_ActiveElement, targetIndex) == true)
                                {
                                    System.Object tempObject = m_ElementList[m_ActiveElement];
                                    for (int i = 0; i < m_ElementList.Count - 1; i++)
                                    {
                                        if (i >= m_ActiveElement)
                                            m_ElementList[i] = m_ElementList[i + 1];
                                    }
                                    for (int i = m_ElementList.Count - 1; i > 0; i--)
                                    {
                                        if (i > targetIndex)
                                            m_ElementList[i] = m_ElementList[i - 1];
                                    }
                                    m_ElementList[targetIndex] = tempObject;
                                }
                            }

                            var oldActiveElement = m_ActiveElement;
                            var newActiveElement = targetIndex;

                            // update the active element, now that we've moved it
                            m_ActiveElement = targetIndex;

                            if (OnReorderDecide?.Invoke(this, m_ActiveElement, targetIndex) == true)
                            {
                                // give the user a callback
                                if (OnReorderCallbackWithDetails != null)
                                    OnReorderCallbackWithDetails(this, oldActiveElement, newActiveElement);
                                else if (OnReorderCallback != null)
                                    OnReorderCallback(this);
                                if (OnChangedCallback != null)
                                    OnChangedCallback(this);
                            }
                        }
                        else
                        {
                            // if mouse up was on the same index as mouse down we fire a mouse up callback (useful if for beginning renaming on mouseup)
                            if (OnMouseUpCallback != null)
                                OnMouseUpCallback(this);
                        }
                    }
                    finally
                    {
                        // It's quite possible a call to EndGUI was made in one of our callbacks
                        // (and thus an ExitGUIException thrown). We still need to cleanup before
                        // we exitGUI proper.
                        GUIUtility.hotControl = 0;
                        m_NonDragTargetIndices = null;
                    }
                    break;
            }
            // if the index has changed and there is a selected callback, call it
            if ((m_ActiveElement != oldIndex || clicked) && OnSelectCallback != null)
                OnSelectCallback(this);
        }

        bool IsMouseInsideActiveElement(Rect listRect)
        {
            int mouseRowIndex = GetRowIndex(Event.current.mousePosition.y - listRect.y);
            return mouseRowIndex == m_ActiveElement && GetRowRect(mouseRowIndex, listRect).Contains(Event.current.mousePosition);
        }

        private void UpdateDraggedY(Rect listRect)
        {
            m_DraggedY = Mathf.Clamp(Event.current.mousePosition.y - listRect.y, m_DragOffset, listRect.height - (GetElementHeight(m_ActiveElement) - m_DragOffset));
        }

        private int CalculateRowIndex()
        {
            return GetRowIndex(m_DraggedY);
        }

        private int GetRowIndex(float localY)
        {
            if (ElementHeightCallback == null)
                return Mathf.Clamp(Mathf.FloorToInt(localY / ElementHeight), 0, count - 1);

            float rowYOffset = 0;
            for (int i = 0; i < count; i++)
            {
                float rowYHeight = ElementHeightCallback(i);
                float rowYEnd = rowYOffset + rowYHeight;
                if (localY >= rowYOffset && localY < rowYEnd)
                {
                    return i;
                }
                rowYOffset += rowYHeight;
            }
            return count - 1;
        }

        private bool IsDragging()
        {
            return m_Dragging;
        }

        public void GrabKeyboardFocus()
        {
            GUIUtility.keyboardControl = id;
        }

        public void ReleaseKeyboardFocus()
        {
            if (GUIUtility.keyboardControl == id)
            {
                GUIUtility.keyboardControl = 0;
            }
        }

        public bool HasKeyboardControl()
        {
            return EditorGUIHelper.HasKeyboardFocus(id);
        }
    }
}
