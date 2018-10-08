# service-fabric-52abp-ocelot
A Service Fabric sample with a Frontend, one API Gateway and 52abp Microservices

## 解决方案内项目说明

### 1 Service Fabric Application -> NanoFabric_ServiceFabric
    ServiceFabric App,可理解为其余所有项目的启动引导


### 2 IdentityServer 4 -> ServiceOAuth 
    IdentityServer4 Server


### 3 Gateway (Ocelot) -> Gateway
    API网关

### 4 Microservices -> 01 - ServiceA -> ServiceA
    default values api
    IdentityServer4 Client:default.client


### 5 Frontend(52ABP) -> LTMCompanyNameFree.YoyoCmsTemplate.Web.Host
  
    52ABP API Host,无状态服务,端口10090
    IdentityServer4 Client:52abp.client


###  5 Frontend(52ABP) -> SPAHost

    52abp前端计算静态站点,无状态服务,端口10091
    运行程序前，使用命令行打开此路径
    1、输入 npm install 还原依赖  或  输入 yarn 进行还原依赖
    2、还原成功后，输入 npm run build 打包


### 5 Frontend(52ABP) -> FrontendConsoleApp

    测试的控制台程序



### 5 Frontend(52ABP) -> 52ABP -> All 
    * 此目录下为 ServiceOAuth 和 LTMCompanyNameFree.YoyoCmsTemplate.Web.Host的依赖

    * ServiceOAuth 依赖于 52ABP 的 User 和登陆等等,其余都不依赖,只是为了临时使用方便

    * LTMCompanyNameFree.YoyoCmsTemplate.Web.Host 依赖所有,并提供所有api

---

## 启动顺序
    1、SPAHost 按步骤还原和发布
    2、启动 NanoFabric_ServiceFabric 
    3、启动成功，访问 http://localhost:10091




