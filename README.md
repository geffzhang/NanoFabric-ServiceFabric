# NanoFabric-ServiceFabric 操作手册

# service-fabric-52abp-ocelot
A Service Fabric sample with a Frontend, one API Gateway and 52abp Microservices
基于Service fabric + Ocelot + Identity Server4 + 52ABP 的案例展示

仓库地址信息：https://github.com/geffzhang/NanoFabric-ServiceFabric
本案例是由[张善友](https://github.com/geffzhang)，[staneee]( https://github.com/staneee)，[梁桐铭](https://github.com/ltm0203) 共同协作打造
 基于Service fabric + Ocelot + Identity Server4 + 52ABP 的案例展示
## 关于service farbic的基础部分参考
https://docs.microsoft.com/zh-cn/azure/service-fabric/service-fabric-overview
以上为 基础内容

## 解决方案内项目说明
![项目截图.png](https://upload-images.jianshu.io/upload_images/1979022-c70a3b9f36e88662.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)



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

## 本地启动项目操作说明
  1、 启动 Service fabric local cluster manager ,保证本地集群是打开的状态。
![image.png](https://upload-images.jianshu.io/upload_images/1979022-0304e8c038f8e992.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
2、 打开`NanoFabric-ServiceFabric.sln`解决方案，**需要使用管理员权限**这点很重要，否则报错。
3、 默认生成数据库内容，`LTMCompanyNameFree.YoyoCmsTemplate.Migrator`启动迁移文件工具，生成数据库。
![image.png](https://upload-images.jianshu.io/upload_images/1979022-50d14583343478f9.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
4、52abp前端计算静态站点,无状态服务,端口10091 运行程序前，使用命令行打开此路径,
需要编译前端包，打开`ClientApp`然后运行
```
1、输入 npm install 还原依赖  或  输入 yarn 进行还原依赖
2、还原成功后，输入 npm run build 打包
```
5、设置`NanoFabric_ServiceFabric `为默认启动项目，然后启动它。
![image.png](https://upload-images.jianshu.io/upload_images/1979022-050d06f724a70d55.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

6、启动成功，访问 http://localhost:10091

![调用的是ocelot的网关](https://upload-images.jianshu.io/upload_images/1979022-63a243a350b1daf2.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

  ## PS: 账号和密码
默认账号：Admin
默认密码：123qwe




