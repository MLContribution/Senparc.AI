﻿using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Planning.Handlebars;
using Senparc.AI.Entities;
using Senparc.AI.Interfaces;
using Senparc.AI.Kernel;
using Senparc.AI.Kernel.Handlers;
using Senparc.AI.Kernel.KernelConfigExtensions;
using Senparc.CO2NET.Extensions;

namespace Senparc.AI.Samples.Consoles.Samples
{
    public class PlanSample
    {
        IAiHandler _aiHandler;

        SemanticAiHandler _semanticAiHandler => (SemanticAiHandler)_aiHandler;
        string _userId = "Jeffrey";

        public PlanSample(IAiHandler aiHandler)
        {
            _aiHandler = aiHandler;
            _semanticAiHandler.SemanticKernelHelper.ResetHttpClient(enableLog: SampleSetting.EnableHttpClientLog);//同步日志设置状态
        }

        public async Task RunAsync()
        {

            while (true)
            {
                await Console.Out.WriteLineAsync("PlanSample 开始运行。请输入需要生成的内容：");


                await Console.Out.WriteLineAsync("请输入");

                var iWantToRun = _semanticAiHandler
                               .IWantTo()
                               .ConfigModel(ConfigModel.Chat, _userId)
                               .BuildKernel();

                //var planner = iWantToRun.ImportPlugin(new TextMemoryPlugin(iWantToRun.Kernel.Memory)).skillList;

                var dir = Path.GetDirectoryName(this.GetType().Assembly.Location);//System.IO.Directory.GetCurrentDirectory();
                                                                                  //Console.WriteLine("dir:" + dir);

                var pluginsDirectory = Path.Combine(dir, "..", "..", "..", "plugins");
                //Console.WriteLine("pluginsDirectory:" + pluginsDirectory);

                await Console.Out.WriteLineAsync("Add Your Plugins, input q to finish");
                var plugin = Console.ReadLine();
                while (plugin != "q")
                {
                    //SummarizePlugin , WriterPlugin , ...
                    //iWantToRun.ImportPluginFromPromptDirectory(Path.Combine(pluginsDirectory, "WriterPlugin"), plugin);
                    //iWantToRun.ImportPluginFromPromptDirectory(Path.Combine(pluginsDirectory, "SummarizePlugin"), plugin);

                    var importResult = iWantToRun.ImportPluginFromPromptDirectory(pluginsDirectory, plugin);
                    plugin = Console.ReadLine();
                }

                await Console.Out.WriteLineAsync("Tell me your task:");
                //Tomorrow is Valentine's day. I need to come up with a few date ideas and e-mail them to my significant other
                var ask = Console.ReadLine();
                await Console.Out.WriteLineAsync();

#pragma warning disable SKEXP0060
                var plannerConfig = new HandlebarsPlannerOptions { /*MaxTokens = 2000,*/ AllowLoops = true };
                var planner = new HandlebarsPlanner(plannerConfig);

                //var ask = "If my investment of 2130.23 dollars increased by 23%, how much would I have after I spent 5 on a latte?";

                var plan = await planner.CreatePlanAsync(iWantToRun.Kernel, ask);

                await Console.Out.WriteLineAsync("Plan:");
                await Console.Out.WriteLineAsync(plan.Prompt);

                await Console.Out.WriteLineAsync();

                await Console.Out.WriteLineAsync("Now system will add a new plan into your request: Rewrite the above in the style of Codeing. Press Enter");

                Console.ReadLine();

                //新建计划，并执行 Plan：

                string prompt = @"
{{$input}}

Rewrite the above in the style of Codeing.
";
                var shakespeareFunction = iWantToRun.CreateFunctionFromPrompt(prompt, "code", "CodingPlugin", maxTokens: 2000, temperature: 0.2, topP: 0.5).function;

                // Execute the new plan

                var newPlan = await planner.CreatePlanAsync(iWantToRun.Kernel, ask);
                await Console.Out.WriteLineAsync("New Plan:");
                await Console.Out.WriteLineAsync(newPlan.Prompt);

                Console.WriteLine("Updated plan:\n");
                // Execute the plan

                var newContext = iWantToRun.CreateNewArguments();//TODO: 直返会一个对象？
                var newResult = await newPlan.InvokeAsync(iWantToRun.Kernel, newContext.arguments);

                Console.WriteLine("Plan results:");
                Console.WriteLine(newResult);
                Console.WriteLine();

                await Console.Out.WriteLineAsync("== plan execute finish ==");

                await Console.Out.WriteLineAsync();

                await Console.Out.WriteLineAsync("输入 exit 退出 Planner 测试，任意内容继续");

                if (Console.ReadLine().ToUpper() == "EXIT")
                {
                    break;
                }
            }
        }

    }


}
