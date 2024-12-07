﻿/**
Last modified: 20231207 - 修复拼写错误和中文编码错误
Last modifier: FelixJ 
*/


using Microsoft.SemanticKernel;
using Senparc.AI.Interfaces;
using Senparc.AI.Kernel.Handlers;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Senparc.AI.Kernel
{
    /// <summary>
    /// Senparc.AI.Kernel 模块的 AI 接口返回信息
    /// </summary>
    public class SenparcAiResult : IAiResult
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual string InputContent { get; set; }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual IAiContext InputContext { get; set; }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual string OutputString { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual Exception? LastException { get; set; }

        public virtual IWantToRun IWantToRun { get; set; }

        public SenparcAiResult(IWantToRun iwantToRun, string? inputContent)
        {
            IWantToRun = iwantToRun;
            InputContent = inputContent;
        }

        public SenparcAiResult(IWantToRun iwantToRun, IAiContext inputContext)
        {
            IWantToRun = iwantToRun;
            InputContext = inputContext;
        }
    }

    public class SenaprcAiResult<T> : SenparcAiResult, IAiResult
    {
        public T Result { get; set; }
        public IAsyncEnumerable<StreamingKernelContent>? /*SKContext*/ StreamResult { get; set; }

        public SenaprcAiResult(IWantToRun iWwantToRun, string inputContent)
            : base(iWwantToRun, inputContent)
        {
        }

        public SenaprcAiResult(IWantToRun iWwantToRun, IAiContext inputContext)
           : base(iWwantToRun, inputContext)
        {
        }
    }

    public class SenparcTextAiResult : SenaprcAiResult<string>, IAiResult
    {
        public string Result { get; set; }

        public SenparcTextAiResult(IWantToRun iWwantToRun, string inputContent)
             : base(iWwantToRun, inputContent)
        {
        }

        public SenparcTextAiResult(IWantToRun iWwantToRun, IAiContext inputContext)
           : base(iWwantToRun, inputContext)
        {
        }
    }

    public class SenparcKernelAiResult : SenparcKernelAiResult<string>, IAiResult
    {
        public SenparcKernelAiResult(IWantToRun iWwantToRun, string? inputContent)
             : base(iWwantToRun, inputContent)
        {
        }

        public SenparcKernelAiResult(IWantToRun iWwantToRun, IAiContext inputContext)
           : base(iWwantToRun, inputContext)
        {
        }
    }

    public class SenparcKernelAiResult<T> : SenaprcAiResult<FunctionResult>, IAiResult
    {
        public T? Output => Result.GetValue<T>();

        public FunctionResult /*SKContext*/ Result { get; set; }
        public IAsyncEnumerable<StreamingKernelContent>? /*SKContext*/ StreamResult { get; set; }

        public SenparcKernelAiResult(IWantToRun iWwantToRun, string? inputContent)
             : base(iWwantToRun, inputContent)
        {

        }

        public SenparcKernelAiResult(IWantToRun iWwantToRun, IAiContext inputContext)
           : base(iWwantToRun, inputContext)
        {
        }
    }
}