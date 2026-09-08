using Assets.Scripts.Framework;
using Assets.Scripts.Framework.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    public Slider progressBar;
    public Text progressText;

    private void Start()
    {
        // 绑定大喇叭事件

        // 进度条变化事件
        HotUpdate.OnProgressUpdate += UpdateUI;
        // 资源更新完成事件
        HotUpdate.OnUpdateComplete += JumpToGame;
    }

    private void OnDestroy()
    {
        HotUpdate.OnProgressUpdate -= UpdateUI;
        HotUpdate.OnUpdateComplete -= JumpToGame;
    }

    // UI变化
    private void UpdateUI(float progressValue, string tipText)
    {
        progressBar.value = progressValue;
        progressText.text = $"{tipText} - {(progressValue * 100):F1}%";
    }

    // 场景加载
    private void JumpToGame()
    {
        AppLog.LogService("[LoadingUI]","热更完毕，切入主游戏场景!");
        // 换成你工程里真实存在的下一个场景名字
        SceneManager.LoadScene("01_MainCity");
    }
}
