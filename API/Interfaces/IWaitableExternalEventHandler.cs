﻿using Autodesk.Revit.UI;

namespace revit_mcp_sdk.API.Interfaces
{
    /// <summary>
    /// 可等待的外部事件处理器接口
    /// </summary>
    public interface IWaitableExternalEventHandler : IExternalEventHandler
    {
        /// <summary>
        /// 等待操作完成
        /// </summary>
        /// <param name="timeoutMs">超时时间（毫秒）</param>
        /// <returns>是否在超时前完成</returns>
        bool WaitForCompletion(int timeoutMs);
    }
}
