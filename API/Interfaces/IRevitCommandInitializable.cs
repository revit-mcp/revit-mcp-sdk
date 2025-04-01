using Autodesk.Revit.UI;

namespace revit_mcp_sdk.API.Interfaces
{
    public interface IRevitCommandInitializable
    {
        /// <summary>
        /// 初始化命令
        /// </summary>
        /// <param name="uiApplication">Revit UI 应用程序</param>
        void Initialize(UIApplication uiApplication);
    }
}
