using Sirenix.Utilities;
using UniTool.Global;

namespace UniTool.Editor.Configs
{
    public class UniToolEditorGlobalConfigAttribute : GlobalConfigAttribute
    {
        public UniToolEditorGlobalConfigAttribute()
            : base(UniToolAssetPaths.EditorConfigsPath)
        {
            
        }  
    }
}
