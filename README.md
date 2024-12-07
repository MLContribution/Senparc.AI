# Senaprc.AI
Senparc 全家桶的 AI 扩展包，目前主要集中于 LLM（大语言模型）的交互。


## 项目介绍

| 名称 | 说明 | Nuget |
|--------|--------|--------|
| Senparc.AI | 为所有标准接口和基础功能的基础模块 | [![Senparc.AI](https://img.shields.io/nuget/v/Senparc.AI.svg)](https://www.nuget.org/packages/Senparc.AI/) |
| Senparc.AI.Kernel | 为基于 Senparc.AI 标准，使用 [SemanticKernel](https://github.com/microsoft/semantic-kernel) 实现的接口调用，可以实现即插即用。|  [![Senparc.AI.Kernel](https://img.shields.io/nuget/v/Senparc.AI.Kernel.svg)](https://www.nuget.org/packages/Senparc.AI.Kernel/) |
| Senparc.AI.Agents | 为基于 Senparc.AI 标准，使用 [AutoGen](https://github.com/microsoft/autogen) 实现的 Agent 集成扩展模块。|  [![Senparc.AI.Agents](https://img.shields.io/nuget/v/Senparc.AI.Agents.svg)](https://www.nuget.org/packages/Senparc.AI.Agents/) |
`Senparc.AI.PromptRange`<br>([独立项目](https://github.com/Senparc/Senparc.AI.PromptRange)) | 为基于 Senparc.AI 标准，为“PromptRange（提示词靶场）”生态提供底层标准支持的底层库。目前已经由 [Senparc.Xncf.PromptRange](https://github.com/NeuCharFramework/NcfPackageSources/tree/master/src/Extensions/Senparc.Xncf.PromptRange) 实现，可用于开发基于 PromptRange 的扩展应用，兼容 Web、桌面、手机等系统（支持 .NET 6.0 及以上框架）。[NeuCharFramework(NCF)](https://github.com/orgs/NeuCharFramework) 框架已经默认集成 [Senparc.Xncf.PromptRange](https://github.com/NeuCharFramework/NcfPackageSources/tree/master/src/Extensions/Senparc.Xncf.PromptRange)，可无需修改任何代码，直接使用。（[什么是 PromptRange？](https://github.com/Senparc/Senparc.AI.PromptRange/wiki/What's-PromptRange%3F)） |  |

## 开发过程

### 第一步：配置账号

在 appsettings.json 中配置 OpenAI 或 Azure OpenAI 的接口信息，如：

``` json
//Senparc.AI 设置
"SenparcAiSetting": {
  "IsDebug": true,
  "AiPlatform": "NeuCharAI", //注意修改为自己平台对应的枚举值
  "NeuCharAIKeys": {
    "ApiKey": "<Your ApiKey>", //在 https://www.neuchar.com/Developer/AiApp 申请
    "NeuCharEndpoint": "https://www.neuchar.com/<DeveloperId>", //查看 ApiKey 时可看到 DeveloperId
    "ModelName": {
      "Chat": "gpt-4o",
      "Embedding": "text-embedding-ada-002",
      "TextCompletion": "gpt-35-turbo-instruct"
    }
  },
  "AzureOpenAIKeys": {
    "ApiKey": "<Your AzureApiKey>", 
    "AzureEndpoint": "<Your AzureEndPoint>", //https://xxxx.openai.azure.com/
    "AzureOpenAIApiVersion": "2022-12-01", //调用限制请参考：https://learn.microsoft.com/en-us/azure/cognitive-services/openai/quotas-limits
    "ModelName": {
      "Chat": "gpt-35-turbo"
    }
  },
  "OpenAIKeys": {
    "ApiKey": "<Your OpenAIKey>",
    "OrganizationId": "<Your OpenAIOrgId>",
    "OpenAIEndpoint": null,
    "ModelName": {
      "Chat": "gpt-35-turbo"
    }
  },
  "HuggingFaceKeys": {
    "Endpoint": "<Your EndPoint>", //HuggingFace 的 Endpoint
    "ModelName": {
      "TextCompletion": "chatglm2"
    }
  },
  "Items": {
    // 更多自定义配置
  }
}
```

其中：`AiPlatform` 是平台类型，目前可选值为 `OpenAI`、`NeuCharAI`、`AzureOpenAI`、`HuggingFace` 和 `FastAPI`，分别对应 openai.com 官方接口（OpenAI）、https://www.neuchar.com 由 Senparc 提供的中转接口、基于微软 Azure 的 AzureOpenAI 接口（AOAI)、HuggingFace 接口和 FastAPI 接口，系统会根据配置自动实现切换，无需在逻辑代码中进行判断。

仅当 `AiPlatform` 设置为 `OpenAI` 时，才需要设置 `OpenAIKeys` 及以下参数。

仅当 `AiPlatform` 设置为 `NeuCharAI` 时，才需要设置 `NeuCharAIKeys` 及以下参数。

仅当 `AiPlatform` 设置为 `AzureOpenAI` 时，才需要设置 `AzureOpenAIKeys` 及以下参数。

其他平台以此类推。

每一个平台类型配置下，都有一个 `ModelName` 节点，用于设置该平台下需要被支持的模型类型，例如需要将 Chat 接口设置为 GPT-4 模型，则设置：`"Chat": "gpt-4"`。

> 提示：AzureOpenAI 调用限制请参考：https://learn.microsoft.com/en-us/azure/cognitive-services/openai/quotas-limits<br>
> OpenAI 调用限制请参考 OpenAI 后台：https://platform.openai.com/docs/guides/rate-limits

#### 进阶（一）：配置多模型环境

#### 进阶（二）：动态配置模型参数

### 第二步：开发

Senparc.AI 使用了创新的对话式编程体验，您无需了解过多不同平台、SDK 的详细用法，只需要按照自己的想法进行定义和编程，最后接收结果，以目前最火的聊天场景（Chat）为例：

```C#
//获取 AI 模型配置（从 appsettings.json 自动读取）
var aiSetting = Senparc.AI.Config.SenparcAiSetting;

// 创建 AI Handler 处理器（也可以通过工厂依赖注入）
var handler = new SemanticAiHandler(aiSetting);

// 定义 AI 接口调用参数和 Token 限制等
var promptParameter = new PromptConfigParameter()
{
    MaxTokens = 2000,
    Temperature = 0.7,
    TopP = 0.5,
};

// 准备运行
var userId = "JeffreySu";//区分用户
var iWantToRun = 
     handler.IWantTo()
            .ConfigModel(aiSetting.ModelName.Chat, userId, modelName)
            .BuildKernel()
            .RegisterSemanticFunction("ChatBot", "Chat", promptParameter)
            .iWantToRun;

// 输入/提问，获取结果
var prompt = "请问中国有多少人口？";
var aiRequest = iWantToRun.CreateRequest(prompt, true, true);
var aiResult = await iWantToRun.RunAsync(aiRequest);
//aiResult.Result 结果：中国的人口约为13.8亿。
```


<img width="623" alt="image" src="https://user-images.githubusercontent.com/2281927/230152103-3486fbfc-2426-407c-bcb6-74d4485eaf91.png">

## 示例

所有快速参考示例位于 `/Samples/` 文件夹内

文件夹 |  说明
------|--------
Samples/Senparc.AI.Samples.Consoles | 命令行
Samples/Senparc.AI.Samples.Agents | Agent（智能体）测试，已集成 AutoGen

## 命令示例行使用说明

### 1. 打开解决方案

打开解决方案 `Senparc.AI.sln`，设置 appsettings.json 中 ApiKey 等平台参数，启动项目 `Senparc.AI.Samples.Consoles`：

<img width="801" alt="image" src="https://github.com/Senparc/Senparc.AI/assets/2281927/d38d15cd-6b88-488e-8b74-a26934dfb538">

### 2. 操作

#### 2.1 对话

输入 `1`，进入对话操作：

<img width="738" alt="image" src="https://github.com/Senparc/Senparc.AI/assets/2281927/32933f21-c8db-4090-ad5a-a955039ee407">

#### 2.2 TextCompletion

在主界面输入 `2` 进入 TextCompletion 操作：

<img width="717" alt="image" src="https://github.com/Senparc/Senparc.AI/assets/2281927/b28e67de-5334-4703-8b74-3da7f9c328fd">


#### 2.3 Embedding

在主界面输入 `3` 即可进入 Embedding 操作，Embedding 分为常规信息和引用信息两类，将在下一步中做选择：

#### 2.3.1 常规 Embedding（Information）

选择 `1`，进入到常规 Embedding 测试，输入信息由 3 个英文冒号分割，录入完成后输入 `n` 开始对话测试：

<img width="708" alt="image" src="https://github.com/Senparc/Senparc.AI/assets/2281927/f22de4b1-1920-46fb-97fa-18265374d2ad">


#### 2.3.2 引用 Embedding（Reference）
2.2.2 上一步选择 `2`，进入到引用 Embedding 测试，输入信息由 3 个英文冒号分割，录入完成后输入 `n` 开始对话测试：

<img width="957" alt="image" src="https://github.com/Senparc/Senparc.AI/assets/2281927/effd6bab-ba23-40ec-a125-7270b4dbbd29">


#### 2.4 DallE 绘图操作

初始界面中输入 `4`，进入 DallE 接口的绘图操作：

<img width="941" alt="image" src="https://github.com/Senparc/Senparc.AI/assets/2281927/e353b059-3399-425a-80f3-ec0eed767b90">

结果将以 URL 的形式返回，此时出入 `s` ，可保存图片到本地：

<img width="951" alt="image" src="https://github.com/Senparc/Senparc.AI/assets/2281927/61daa2e3-a781-40ea-b47b-c360a3cc4464">

> 注意：接口返回的 URL 是一个暂存地址，不可用于持久化的展示，需要及时保存，

## 单元测试

<img width="498" alt="image" src="https://github.com/Senparc/Senparc.AI/assets/2281927/6ec61a15-e1c7-423b-9886-0e401c070e35">

## TODO:
1. [x] 实现更多模型和模式的匹配。
1. [x] 实现全自动的工厂模块自动配置。
1. [x] 集成到 [Senaprc.Weixin SDK](https://github.com/JeffreySu/WeiXinMPSDK)，0 逻辑代码实现 AI 能力接入（聊天场景为主）。
1. [x] 集成到 [NeuCharFramework](https://github.com/NeuCharFramework/NCF)，0 逻辑代码实现 AI 能力接入（开发和云运营场景为主）。
1. [ ] 完成更多默认模型适配（目前已经提供自定义扩展能力）
1. [ ] 完善独立文档
