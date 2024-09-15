using Sirenix.Utilities;

namespace UniTool.Editor.Configs
{
    public class UniToolEditorGlobalConfigAttribute : GlobalConfigAttribute
    {
        public UniToolEditorGlobalConfigAttribute()
            : base(UniToolEditorAssetsPath.ConfigsPath)
        {
        }  
    }
}
