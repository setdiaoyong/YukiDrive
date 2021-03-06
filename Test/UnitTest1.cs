using System.Threading;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YukiDrive.Helpers;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using YukiDrive.Services;
using YukiDrive.Contexts;
using YukiDrive;
using Newtonsoft.Json;
using static YukiDrive.Helpers.ProtectedApiCallHelper;

namespace Test
{
    /// <summary>
    /// 缓存Token到硬盘
    /// </summary>
    static class TokenCacheHelper
    {
        public static void EnableSerialization(ITokenCache tokenCache)
        {
            tokenCache.SetBeforeAccess(BeforeAccessNotification);
            tokenCache.SetAfterAccess(AfterAccessNotification);
        }

        /// <summary>
        /// Path to the token cache
        /// </summary>
        public static readonly string CacheFilePath = "/Users/yukino/Desktop/Token/" + "TokenCache.bin";

        private static readonly object FileLock = new object();


        private static void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (FileLock)
            {
                args.TokenCache.DeserializeMsalV3(System.IO.File.Exists(CacheFilePath)
                        ? System.IO.File.ReadAllBytes(CacheFilePath)
                        : null);
            }
        }

        private static void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (args.HasStateChanged)
            {
                lock (FileLock)
                {
                    // reflect changesgs in the persistent store
                    System.IO.File.WriteAllBytes(CacheFilePath, args.TokenCache.SerializeMsalV3());
                }
            }
        }
    }


    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestConfig(){
            //Debug.WriteLine(System.IO.Directory.GetCurrentDirectory());
            Debug.WriteLine(Guid.NewGuid().ToString());
        }

        [TestMethod]
        public void TestJson()
        {
            while (true)
            {

                //Debug.WriteLine(Configuration.builder.);
                Thread.Sleep(1000);
            }
        }
        [TestMethod]
        public void TestMethod1()
        {
            Debug.WriteLine(System.IO.Directory.GetCurrentDirectory());
            DriveAccountService driveAccountService = new DriveAccountService(new SiteContext(),new TokenService());
            DriveService service = new DriveService(driveAccountService, new SiteContext(), new DriveContext());
            //var drive = driveAccountService.Graph.Sites["alphaone.sharepoint.cn,217fb322-ee69-4fe7-af84-a1d018d1cf2b,c5bc7d0b-0a0f-49f6-9589-61481d4266ab"].Drive.Request().GetAsync().Result;
            //Debug.WriteLine(driveAccountService.Graph.Sites["alphaone.sharepoint.cn,217fb322-ee69-4fe7-af84-a1d018d1cf2b,c5bc7d0b-0a0f-49f6-9589-61481d4266ab"].Drive.RequestUrl);
            // string uploadStr = driveAccountService.Graph.Me.Drive.Root.ItemWithPath("TestUpload/TestUpload").CreateUploadSession().Request().RequestUrl;
            // Debug.WriteLine(uploadStr);
            string token = driveAccountService.GetToken();
            Debug.WriteLine(token);
            var result = driveAccountService.Graph.Me.Drive.Root.ItemWithPath("TestUpload/photo7.jpeg").Thumbnails.Request().GetAsync().Result;
            // ProtectedApiCallHelper apiCallHelper = new ProtectedApiCallHelper(new HttpClient());
            // apiCallHelper.CallWebApiAndProcessResultASync(uploadStr,token,o => {
            //     Debug.WriteLine(o["uploadUrl"]);
            // },Method.Post).Wait();
            // Debug.WriteLine(service.GetUploadUrl("615.png"));
        }

        [TestMethod]
        public void JsonTest()
        {
            string json = @"{
  'error': {
    'code': 'itemNotFound',
    'message': 'Requested site could not be found',
    'innerError': {
      'request-id': 'c19041e9-3c92-4551-a41d-88014ac1e0d1',
      'date': '2020-03-18T18:11:34'
    }
  }
}";
        JObject jo = JsonConvert.DeserializeObject(json) as JObject;
        Debug.WriteLine(jo.Property("error").Value["message"].ToString());
        }
        [TestMethod]
        public void TestTimer(){
            Timer timer = new Timer(o => {
                Debug.WriteLine("计时计时");
            },null,TimeSpan.FromSeconds(0),TimeSpan.FromHours(1));
            while(true){
                Thread.Sleep(1000);
            }
        }
        IConfidentialClientApplication app;
        public void InitDrive()
        {
            app = ConfidentialClientApplicationBuilder
            .Create("65a90f7f-dffc-421e-8d49-2be126e80ba6")
            //.WithTenantId(tenantID)
            .WithClientSecret("/8AO9lK-t=7?mzDxFKTDAUU39v2mg@ps")
            .WithRedirectUri("https://localhost:7777")
            .Build();
            //缓存Token
            TokenCacheHelper.EnableSerialization(app.UserTokenCache);
        }
        string[] scopes = new string[] { "Files.ReadWrite.All" };
        [TestMethod]
        public void OnedriveFile()
        {
            // //天朝国情
            // HttpClient.DefaultProxy = new WebProxy("http://127.0.0.1:7890");
            // InitDrive();
            // //Debug.WriteLine(app.GetAuthorizationRequestUrl(scopes).ExecuteAsync().Result.AbsoluteUri);
            // AuthorizationCodeProvider authProvider = new AuthorizationCodeProvider(app);
            // //var result = authProvider.ClientApplication.AcquireTokenByAuthorizationCode(scopes, "OAQABAAIAAABeAFzDwllzTYGDLh_qYbH8L7gUEfgmy7oOvDXlHN45HZz4OzLZa1OMywteS-aSzSXRrXLDvY_I7wqI3xNcQ-JqTvWrZzkeapNv6ayKuVPkNFrJ7ScQox-FLiSK_sHrW_TtDWY_3OKnOlI59WakzlrzsbJzafJlfwS_hir6IrgU55ouIRP0Q-wS1WkAhTbYI1_KmW1ZEDTEBXIgxVUOOxYdei4jvP6lQ3r6PMPzIZj2ytfb0vcr4cwiyjTb38a3HvNtiAmqtO3cL4iOZ4z6WVFtBNF3puFaphHvq42Q9d99Az-I6gnUNuO3w7NvLcALdfDEvAi5NlF_DBfw2UD2VMJ4l5uK0ldFzgaksT5T4Cg57XUf9mH-XWxGtPo1REwsyq4iRLSRphSGCaUMoc6C2UXGD4dBnTYh6SJxOjF_tYqDiuv7dja0njaadsnWcdxy1uXP6Dz7RP-I7bUKKzjkpZF7IpTRi1kfAF5ttslMIdMPp4fnJtnNGF40m7fT5AWm7I2oqUvBs9HR2-LupHoT6ZekZD-YNbsCTO0x_lJuSTyTU6nTZnkl2thhfXExSvdt7Sw_M1eN__MkxMlWo9XsKBJyFXSUB6HnELI10PSreifqSmwovNj5kN07ODzOraEvVgKTH5QmPChvAypcZOS6B3hz-Mq5O3nudhKp0UQerMwlOyAA").ExecuteAsync().Result;
            // var result = authProvider.ClientApplication.AcquireTokenSilent(scopes, "Sakura@yukistudio.onmicrosoft.com").ExecuteAsync().Result;
            // //Debug.WriteLine(result.AccessToken);
            // string siteId = getSiteId(result);
            // //GraphServiceClient graph = new GraphServiceClient(authProvider);

            // //启用代理
            // // Configure your proxy
            // var httpClientHandler = new HttpClientHandler
            // {
            //     Proxy = new WebProxy("http://127.0.0.1:7890"),
            //     UseDefaultCredentials = true
            // };
            // var httpProvider = new HttpProvider(httpClientHandler, false);
            // GraphServiceClient graph = new GraphServiceClient(authProvider,httpProvider);

            // //获取根
            // var drive = graph.Sites[siteId].Drive;
            // YukiDrive.Services.DriveService onedrive = new YukiDrive.Services.DriveService(new YukiDrive.Services.DriveAccountService(new YukiDrive.Contexts.SiteContext()));
            // var files = onedrive.GetDriveItemsByPath("TestDiretory").Result;
            // //var files = onedrive.GetDriveFiles("014CGQVIS74I522TCE6NGYOA3UMWXILLCW").Result;
            // foreach (var item in files)G
            // {
            //     Debug.WriteLine(item.DownloadUrl);
            // }
            // // var item = onedrive.GetDriveItemByPath("TestDiretory/B825BE8D-330C-40D2-BE0C-CE4C408107F8.png").Result;
            // // Debug.WriteLine(item.DownloadUrl);
        }

        // private static string getSiteId(AuthenticationResult result)
        // {
        //     using (HttpClient httpClient = new HttpClient())
        //     {
        //         httpClient.Timeout = TimeSpan.FromSeconds(10);
        //         var apiCaller = new ProtectedApiCallHelper(httpClient);
        //         string siteId = "";
        //         apiCaller.CallWebApiAndProcessResultASync("https://graph.microsoft.com/v1.0/sites/yukistudio.sharepoint.com:/sites/test", result.AccessToken, (result) =>
        //         {
        //             siteId = result.Properties().Single((prop) => prop.Name == "id").Value.ToString();
        //         }).Wait();
        //         return siteId;
        //     }
        // }

        /// <summary>
        /// Display the result of the Web API call
        /// </summary>
        /// <param name="result">Object to display</param>
        private static void Display(JObject result)
        {
            foreach (JProperty child in result.Properties().Where(p => !p.Name.StartsWith("@")))
            {
                Debug.WriteLine($"{child.Name} = {child.Value}");
            }
        }
        public void DoAsync()
        {

            // ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);
            // Debug.WriteLine(authProvider.Scope);
        }

        [TestMethod]
        public void TestTimer2(){
            //定时更新Token
            Timer timer = new Timer(o =>
            {
                // if (File.Exists(TokenCacheHelper.CacheFilePath))
                // {
                //     authorizeResult = authProvider.ClientApplication.AcquireTokenSilent(Configuration.Scopes, Configuration.AccountName).ExecuteAsync().Result;
                // }
                Debug.WriteLine("计时");
            }, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(2));
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
