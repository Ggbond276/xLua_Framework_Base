using Assets.Scripts.Framework.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking; // 网络请求必须引入这个命名空间

namespace Assets.Scripts.Framework
{
    class HotUpdate : MonoBehaviour
    {
        internal class DownFileInfo
        {
            public string url;
            public string fileName;
            public DownloadHandler fileData;
        }
 
        public readonly string FileListName = "FileList.txt";
        public readonly string ServerUrl;
        /// <summary>
        /// 只读区域文件列表二进制文件
        /// </summary>
        public byte[] m_ReadPathFileListData;
        /// <summary>
        /// 沙盒区域文件列表二进制文件
        /// </summary>
        public byte[] m_ServerFileListData;


        /// <summary>
        /// 热更状态机入口：判断环境，分流执行出厂释放或云端比对
        /// </summary>
        private void Start()
        {
            AppLog.LogSys("HotUpdate", "模块初始化，准备执行状态机...");

            if(IsFirstInstall())
            {
                AppLog.LogSys("HotUpdate", "沙盒区未检出 FileList.txt。判定为首次启动，开启本地资源释放管线 (Unpack)...");
                ReleaseResources();
            }  else
            {
                AppLog.LogSys("HotUpdate", "沙盒区环境校验通过。跳过释放，转入云端版本比对管线 (CheckUpdate)...");
                CheckUpdate();
            }
        }

        /// <summary>
        /// 环境探针：探测沙盒区是否丢失 FileList 字典，以判定是否为首次安装
        /// </summary>
        /// <returns>如果是首次安装返回 true</returns>
        private bool IsFirstInstall()
        {
            
            string readPath = Path.Combine(PathUtil.StreamingAssetsPath, FileListName);
            bool isExistsReadPath = FileUtil.IsExists(readPath);

            string readWritePath = Path.Combine(Application.persistentDataPath, FileListName);
            bool isExistsReadWritePath = FileUtil.IsExists(readWritePath);

            // 探针属于底层物理硬盘扫描，使用 IO 金色日志
            AppLog.LogIO("HotUpdate IO", $"环境探针 -> 只读区: {isExistsReadPath}, 沙盒区: {isExistsReadWritePath}");

            return isExistsReadPath && !isExistsReadWritePath;
        }

        /// <summary>
        /// 释放管线起点：发起提取只读区 (StreamingAssets) 初始字典的网络请求
        /// </summary>
        private void ReleaseResources()
        {
            string url = Path.Combine(PathUtil.StreamingAssetsPath, FileListName);

            DownFileInfo info = new DownFileInfo { url = url };


            AppLog.LogSys("HotUpdate Unpack", "正在读取光盘原始字典...");

            // 这一步执行完成之后 所有的二进制数据就全部进到info中了
            StartCoroutine(DownloadFile(info, OnDownloadReadPathFileListComplete));
        }

        /// <summary>
        /// 释放管线：初始字典提取完毕，缓存锚点数据，并开启批量出厂资源拷贝协程
        /// </summary>
        /// <param name="file">包含光盘字典文本与字节流的数据包</param>
        private void OnDownloadReadPathFileListComplete(DownFileInfo file)
        {
            m_ReadPathFileListData = file.fileData.data;

            // 这一步在干什么
            List<DownFileInfo> fileInfos = GetFileList(file.fileData.text, PathUtil.StreamingAssetsPath);

            AppLog.LogSys("HotUpdate Unpack", $"光盘字典解析成功，共需释放 {fileInfos.Count} 个资源包。开始批量提取...");

            StartCoroutine(DownloadFile(fileInfos, OnReleaseFileComplete, OnReleaseAllFileComplete));
        }

        /// <summary>
        /// 释放管线：单个出厂资源包提取完成，将其强制写入本地沙盒
        /// </summary>
        /// <param name="fileInfo">包含单个 .ab 包二进制数据的数据包</param>
        private void OnReleaseFileComplete(DownFileInfo fileInfo)
        {
            // 找到沙盒地址
            string writeFile = Path.Combine(Application.persistentDataPath, fileInfo.fileName);
            // 将文件写入沙盒
            FileUtil.WriteFile(writeFile, fileInfo.fileData.data);

            AppLog.LogIO("HotUpdate Unpack", $"[释放成功] {fileInfo.fileName} -> 沙盒");
        }


        /// <summary>
        /// 释放管线终点：全量出厂包拷贝完毕，写入字典存档点，无缝衔接至更新管线
        /// </summary>
        private void OnReleaseAllFileComplete()
        {
            string dictPath = Path.Combine(Application.persistentDataPath, FileListName);
            FileUtil.WriteFile(dictPath, m_ReadPathFileListData);

            AppLog.LogSys("HotUpdate Unpack", "出厂资源全量释放完毕！存档点已建立!");

            CheckUpdate();
        }

        /// <summary>
        /// 核心解析器：将多行的 FileList.txt 文本转换为待下载的数据包裹列表
        /// </summary>
        /// <param name="fileData">原始字典文本内容</param>
        /// <param name="path">资源根目录网络绝对路径</param>
        /// <returns>待处理的包裹集合</returns>
        private List<DownFileInfo> GetFileList(string fileData, string path)
        {
            string content = fileData.Trim().Replace("\r", "");
            string[] files = content.Split('\n');

            List<DownFileInfo> downFileInfos = new List<DownFileInfo>(files.Length);

            for(int i = 0; i < files.Length; i++)
            {
                if (string.IsNullOrEmpty(files[i])) continue;

                string[] info = files[i].Split("|");

                DownFileInfo downFileInfo = new DownFileInfo();
                downFileInfo.fileName = info[1];
                downFileInfo.url = Path.Combine(path, info[1]);

                downFileInfos.Add(downFileInfo);
            }
            return downFileInfos;
        }


        /// <summary>
        /// 更新管线起点：发起向云端资源服务器请求最新版本字典的网络请求
        /// </summary>
        private void CheckUpdate()
        {
            string url = Path.Combine(ServerUrl, FileListName);
            DownFileInfo downFileInfo = new DownFileInfo { url = url };
            AppLog.LogNet("HotUpdate Check", $"正在向服务器请求最新版本字典: {url}");

            // 下载方法会将内容下载到DownFileInfo 这个快递包中 然后讲快递包交给回调函数处理
            StartCoroutine(DownloadFile(downFileInfo, OnDownloadServerFileListComplete));
        }

        /// <summary>
        /// 更新管线：云端字典下载完毕，执行本地沙盒差异比对 (Diff)，发起增量补丁下载
        /// </summary>
        /// <param name="file">包含云端最新字典文本与字节流的数据包</param>
        private void OnDownloadServerFileListComplete(DownFileInfo file)
        {
            m_ServerFileListData = file.fileData.data;
            // 存放云端下载下来的资源
            List<DownFileInfo> serverFileInfos = GetFileList(file.fileData.text, ServerUrl);
            // 存放比对后需要更新的资源
            List<DownFileInfo> downListFiles = new List<DownFileInfo>();

            AppLog.LogSys("HotUpdate Check", "云端字典解析完毕，开始与本地沙盒进行逐一比对...");

            // 逐一进行文件比对
            for(int i = 0; i < serverFileInfos.Count; i++)
            {
                // 拼接本地文件路径
                string localFile = Path.Combine(Application.persistentDataPath, serverFileInfos[i].fileName);
                // 如果本地不存在这个文件
                if (!FileUtil.IsExists(localFile)) {
                    serverFileInfos[i].url = Path.Combine(ServerUrl, serverFileInfos[i].fileName);
                    downListFiles.Add(serverFileInfos[i]);
                }
            }

            if(downListFiles.Count > 0)
            {
                AppLog.LogSys("HotUpdate Check", $"比对完成！发现 {downListFiles.Count} 个资源需要更新...");

                // 下载完一个文件需要执行OnUpdateFileComplete回调
                // 下载完全部文件需要执行OnUpdateAllFileComplete回调
                StartCoroutine(DownloadFile(downListFiles, OnUpdateFileComplete, OnUpdateAllFileComplete));
            }


        }

        /// <summary>
        /// 底层网络工具：发起单文件 HTTP/File 请求拉取二进制流
        /// </summary>
        /// <param name="info">待下载的包裹信息，需包含 url</param>
        /// <param name="Complete">单个文件拉取落地后的回调触发器</param>
        IEnumerator DownloadFile(DownFileInfo info, Action<DownFileInfo> Complete)
        {
            if (!info.url.StartsWith("http") && !info.url.StartsWith("file"))
            {
                info.url = "file:///" + info.url;

                // 构建并发起网络请求 等待response返回
                UnityWebRequest webRequest = UnityWebRequest.Get(info.url);
                yield return webRequest.SendWebRequest();

                if(webRequest.result == UnityWebRequest.Result.ConnectionError || 
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    AppLog.LogError("HotUpdate Net", $"下载致命错误: {info.url} | 报错: {webRequest.error}");
                    yield break;
                }

                // 签收下载数据
                info.fileData = webRequest.downloadHandler;
                Complete?.Invoke(info);

                // 5. 释放内存
                webRequest.Dispose();
            }
        }
        /// <summary>
        /// 底层网络工具：协程串行调度器，排队执行大宗文件批量拉取
        /// </summary>
        /// <param name="infos">购物车：需要拉取的包裹集合</param>
        /// <param name="Complete">单个文件拉取完毕的回调 (每件触发 1 次)</param>
        /// <param name="DownloadAllComplete">全队列拉取完毕的终极回调 (共触发 1 次)</param>
        IEnumerator DownloadFile(List<DownFileInfo> infos, Action<DownFileInfo> Complete, Action DownloadAllComplete)
        {
            foreach(DownFileInfo info in infos)
            {
                yield return StartCoroutine(DownloadFile(info, Complete));
            }

            DownloadAllComplete?.Invoke();
        }


        /// <summary>
        /// 更新管线：单个云端补丁包下载完成，强制覆盖本地沙盒中的旧文件
        /// </summary>
        /// <param name="fileInfo">满载新版二进制流的数据包</param>
        private void OnUpdateFileComplete(DownFileInfo fileInfo)
        {
            // 拼接文件写入路径
            string writeFile = Path.Combine(Application.persistentDataPath, fileInfo.fileName);
            // 将数据写入该路径
            FileUtil.WriteFile(writeFile, fileInfo.fileData.data);

            AppLog.LogIO("HotUpdate Check", $"[更新覆盖] {fileInfo.fileName} -> 沙盒");
        }

        /// <summary>
        /// 更新管线终点：所有补丁更新完毕，写入最新云端字典，刷新本地版本锚点
        /// </summary>
        private void OnUpdateAllFileComplete()
        {
            // 拼接文件写入路径
            string dictPath = Path.Combine(Application.persistentDataPath, FileListName);
            FileUtil.WriteFile(dictPath, m_ServerFileListData);

            AppLog.LogSys("HotUpdate Check", "所有补丁更新完毕！沙盒字典已刷新至最新版！");
        }

        /// <summary>
        /// 全局管线终点：热更逻辑闭环，拨动底层路由开关至沙盒模式，进入游戏主业务流
        /// </summary>
        void EnterGame()
        {
            PathUtil.IsOnlineUpdateMode = true;

            AppLog.LogSys("HotUpdate End", "热更全管线结束，已切断只读区路由，正式进入游戏主逻辑！");
        }
    }
}
