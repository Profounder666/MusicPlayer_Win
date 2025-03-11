using System;
using System.Collections;
//using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using NAudio.Wave;

public class DownPanel : MonoBehaviour
{
    private Transform start;//先定义几个子节点
    private Transform pause;
    private Transform left;
    private Transform right;
    private Transform volume;//这里我将volume设为音乐源
    private Transform mute;
    private Transform mode;

    static private Text twoName;
    static private Text time;

    //以下与播放音频文件相关
    static public AudioSource audioSource;
    static public CustomSlider slider;
    static public AudioClip audioClip;
    public CustomSlider customSlider;
    static public Transform volumeSlider;
    static public Text modeText;

    // Start is called before the first frame update
    void Start()
    {
        //子节点初始化
        start = transform.Find("Start");
        pause = transform.Find("Pause");
        pause.gameObject.SetActive(false);//先设置为false
        volume = transform.Find("Volume");
        mute = transform.Find("Mute");
        //mute.gameObject.SetActive(false);
        left = transform.Find("Left");
        right = transform.Find("Right");
        mode = transform.Find("Mode");
        //赋予初始值
        twoName = transform.Find("Name").GetComponent<Text>();
        time = transform.Find("Time").GetComponent<Text>();
        twoName.text = "XX-XX";
        time.text = DurationTransform(0f);

        //audioSource = transform.Find("Volume").GetComponent<AudioSource>();
        slider = transform.Find("ProgressBar").GetComponent<CustomSlider>();
        audioSource = GetComponent<AudioSource>();
        volumeSlider = transform.Find("Volume").Find("VolumeSlider");
        modeText = transform.Find("Mode").Find("Text").GetComponent<Text>();
        transform.Find("Mode").Find("Text").gameObject.SetActive(false);
        audioSource.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        //TotalState();//是否可以获得一个每秒调用呢？
        //transform.gameObject.SetActive(true);
        OnProgressChanged();
    }

    
    //实现：时长转化，返回我们看到的字符串类型
    private string DurationTransform(float origin)
    {
        string result = "";
        int min = 0;//定义分钟
        int second = 0;//定义秒
        int current = (int)Math.Ceiling(origin);
        while (current >= 60)
        {
            current -= 60;
            min++;
        }
        second = current;
        //下面进行转化-默认认为歌曲min不超过2位数(这里分和秒都用2位数表示)
        if (second < 10)//0-9
        {
            result = "0" + min.ToString() + ":" + "0" + second.ToString();
        }
        else//10-59
        {
            result = "0" + min.ToString() + ":" + second.ToString();
        }
        return result;
    }

    //实现：下框中最综合的一个方法-以一首歌的状态形式呈现
    public void TotalState()//假设组值已经被传过来了//3
    {
        twoName.text = CenterPanel.groupValues.SongName + "-" + CenterPanel.groupValues.SingerName;
        time.text = "00:00/" + DurationTransform(CenterPanel.groupValues.Duration);
        //if (audioSource.clip == null)
        //{
        //    Mp3ToClip();
        //}
        Mp3ToClip();
    }
    private void CopyToRes(string fileName)
    {
        // 指定音频文件的完整路径 
        fileName += ".mp3";
        string filePath = Application.dataPath + "/ToPutMusic/" + fileName;
        Debug.Log(filePath);
        // 检查文件是否存在 
        if (File.Exists(filePath))
        {
            // 将文件复制到 Resources 文件夹 
            string resourcesPath = Path.Combine(Application.dataPath, "Resources");
            string destinationPath = Path.Combine(resourcesPath,fileName);

            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
                Debug.Log(resourcesPath);
            }

            File.WriteAllBytes(destinationPath, File.ReadAllBytes(filePath));
        }
    }//这个方法太慢了，加载要很久
    //清理resources里的缓存文件
    private void ClearRes()
    {
        // 获取 Resources 文件夹路径 
        string resourcesPath = Path.Combine(Application.dataPath, "Resources");

        // 遍历并删除 Resources 文件夹中的所有文件 
        if (Directory.Exists(resourcesPath))
        {
            foreach (string file in Directory.GetFiles(resourcesPath))
            {
                File.Delete(file);
            }
        }
    }
    private void Mp3ToClip()
    {
        string url = Path.Combine(Application.persistentDataPath,"ToPutMusic",CenterPanel.groupValues.NodeName+".mp3");// 存储路径
        byte[] bytes = GetMP3Bytes(url); // 获取MP3文件的字节数组

        File.WriteAllBytes(url, bytes);  // 写入文件
        //Debug.Log(CenterPanel.groupValues.NodeName);//测试用
        //Debug.Log(transform.gameObject.activeSelf);
        if (transform.gameObject.activeSelf)
        {
            //Debug.Log("已调用协程");
            GameCoroutine.Instance.StartCoroutine(BinaryToClip(url,bytes));
        }
        else
        {
            return;
        }
    }
    byte[] GetMP3Bytes(string path)
    {
        if (File.Exists(path))
        {
            Debug.Log("文件已转为二进制流");
            return File.ReadAllBytes(path);
        }
        else
        {
            Debug.Log("文件不能转为二进制流");
            return new byte[0];
        }
    }
    IEnumerator BinaryToClip1(string filePath)
    {
        // 将本地文件路径转换为 URI 格式
        string uri = "file://" + filePath;
        Debug.Log(uri);
        // 使用 UnityWebRequest 下载音频文件并转换为 AudioClip
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.MPEG))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(uwr.error);
            }
            else
            {
                // 获取 AudioClip
                AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                if (clip != null)
                {
                    Debug.Log("成功获取clip！");
                    //AudioSource audioSource = GetComponent<AudioSource>();
                    //if (audioSource != null)
                    //{
                    //    audioSource.clip = clip;
                    //    //audioSource.Play();
                    //    Debug.Log("clip获取成功");
                    //}
                    //else
                    //{
                    //    Debug.LogError("AudioSource 组件未找到！");
                    //}
                }
                else
                {
                    Debug.LogError("无法获取 AudioClip！");
                }
            }
        }
    }
    IEnumerator BinaryToClip(string filePath, byte[] bytes) {
        try
        {//路径一定要考虑后缀扩展名！！！
            // 使用 MemoryStream 和 Mp3FileReader 解码 MP3 数据
            using (MemoryStream mp3Stream = new MemoryStream(bytes))
            using (Mp3FileReader mp3Reader = new Mp3FileReader(mp3Stream))
            //using (var mp3Reader = new MediaFoundationReader(filePath))
            using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(mp3Reader))
            {
                // 获取音频信息
                int sampleRate = pcmStream.WaveFormat.SampleRate;
                int channels = pcmStream.WaveFormat.Channels;
                long sampleCount = pcmStream.Length / (channels * (pcmStream.WaveFormat.BitsPerSample / 8));

                // 创建 AudioClip-clip文件名为ConvertedAudio
                audioClip = AudioClip.Create(CenterPanel.groupValues.SongName, (int)sampleCount, channels, sampleRate, false);

                // 读取 PCM 数据并设置到 AudioClip
                float[] samples = new float[sampleCount * channels];
                byte[] buffer = new byte[pcmStream.Length];
                int bytesRead = pcmStream.Read(buffer, 0, buffer.Length);
                if (bytesRead != buffer.Length)
                {
                    Debug.LogError("未读取全部数据！");//这里已经读取全部数据
                    yield break;
                }

                for (int i = 0; i < bytesRead / 2; i++)
                {
                    short sample = BitConverter.ToInt16(buffer, i * 2);
                    samples[i] = sample / 32768.0f;
                }
                audioClip.SetData(samples, 0);

                //Debug.Log("clip创建成功");
                // 播放音频
                audioSource = GetComponent<AudioSource>();
                //AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource != null || this.gameObject.activeInHierarchy)
                {
                    audioSource.clip = audioClip;//因为其是一个临时文件，所以在可视化组件中可能不会显示
                    Debug.Log("音频导入成功!"+audioSource.clip.name);
                    //AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
                    //audioSource.Play();//会报错-禁用
                    //Debug.Log(audioSource.isPlaying);
                }
                else
                {
                    Debug.Log("AudioSource 组件未找到！");
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log($"音频转换失败: {ex.Message}");
        }
        yield return null;
    }
    ///<summary>
    ///点播放按钮进行播放
    ///</summary>
    public void OnClickPlay()
    {
        //判断clip是否已经载入source
        if (audioSource.clip != null)
        {
            audioSource.volume = 0.05f;
            audioSource.Play();
            start.gameObject.SetActive(false);
            pause.gameObject.SetActive(true);
            
        }
    }
    public void OnClickPause()
    {
        if (audioSource.clip != null)
        {
            audioSource.Pause();
            start.gameObject.SetActive(true);
            pause.gameObject.SetActive(false);
        }
    }
    static float originVolume;
    public void OnClickMute()
    {
        if (audioSource.clip != null)
        {
            //先获取用户原来的音量大小
            if (audioSource.volume!=0)//非静音状态
            {
                DownPanel.originVolume = audioSource.volume;
                audioSource.volume = 0f;
            }
            else
            {
                audioSource.volume = DownPanel.originVolume;
            }
        }
    }
    /// <summary>
    /// 处理 Slider 值变化事件，调整音频播放时间 
    /// </summary>
    /// <param name="value">Slider 的当前值</param>
    static public bool isDraggingProgress = false;//拖动状态标记
    private void OnProgressChanged()
    {
        if (audioSource.isPlaying&&!isDraggingProgress)
        {
            // 将 Slider 的值设置为音频的当前时间 
            slider.maxValue = CenterPanel.groupValues.Duration;
            //Debug.Log(slider.maxValue);
            slider.value = audioSource.time;

        }
    }
    public void WhenValueChanged()
    {
        time.text = DurationTransform(audioSource.time) + "/" + DurationTransform(slider.maxValue);
    }
    ///<summary>
    ///控制音量的大小
    ///</summary>
    public void OnClickVolume()
    {
        CustomSlider slider = volumeSlider.GetComponent<CustomSlider>();
        slider.maxValue = 0.1f;
        slider.value = audioSource.volume;
        //Debug.Log(volumeSlider.gameObject.activeSelf);
        if (volumeSlider.gameObject.activeSelf==false)//如果隐藏
        {
            volumeSlider.gameObject.SetActive(true);
        }
        else
        {
            volumeSlider.gameObject.SetActive(false);
        }
    }
    public void WhenVolumeChanged()
    {
        audioSource.volume = volumeSlider.GetComponent<CustomSlider>().value;
    }
    public void OnClickMode()
    {
        Transform testNode = mode.Find("Text");
        if (testNode.gameObject.activeSelf == false)
        {
            testNode.gameObject.SetActive(true);
        }
        else
        {
            testNode.gameObject.SetActive(false);
        }
        //audioSource.loop = true;
        //modeText.text = "循环播放(其他播放方式敬请期待)";
    }
    public void OnClickClose()
    {
        Application.Quit();
        Debug.Log("退出成功");
        // 在 Windows 上可以使用以下代码强制退出 
//#if UNITY_STANDALONE_WIN
//        System.Diagnostics.Process.GetCurrentProcess().Kill();
//#endif
    }
    
    //IEnumerator DoVioceClip(string url)//声明一个返回类型为IEnumerator的协程方法，接受一个字符串参数VoiceUrl
    //{
    //    //创建一个UnityWebRequest对象uwr，用于从指定的URL下载MP3格式的音频剪辑。AudioType.MPEG指定下载的是MP3格式的音频
    //    UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
    //    yield return uwr.SendWebRequest();
    //    //if (uwr.isNetworkError)//过时
    //    if(uwr.result==UnityWebRequest.Result.ConnectionError)
    //        Debug.LogError(uwr.error);
    //    else
    //    {
    //        AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
    //        Debug.Log("转化成功");
    //        //AS.clip = clip;
    //        //AS.Play();
    //    }
    //}
    //private void LoadToSource(string filePath)
    //{
    //    if (File.Exists(filePath))
    //    {
    //        // 使用文件路径加载音频数据 
    //        AudioClip clip;
    //        clip = AudioClip.LoadAudioData(filePath, AudioType.MPEG);

    //        if (clip != null)
    //        {
    //            // 将 AudioClip 分配到 AudioSource 组件 
    //            GetComponent<AudioSource>().clip = clip;
    //            Debug.Log("成功加载: " + filePath);
    //        }
    //        else
    //        {
    //            Debug.LogError("无法加载 AudioClip: " + filePath);
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("音频文件不存在: " + filePath);
    //    }
    //}
    /// <summary>
    /// 定时更新 Slider 的协程方法
    /// </summary>
    /// <returns></returns>
    //private IEnumerator UpdateProgressCoroutine()
    //{
    //    while (true)
    //    {
    //        // 获取当前播放时间 
    //        float currentTime = audioSource.time;
    //        // 更新 Slider 的值 
    //        slider.value = currentTime;
    //        // 等待一秒钟后再次执行 
    //        yield return new WaitForSeconds(1f);
    //    }
    //}

    ///// <summary>
    ///// 重置 Slider 和音频播放 
    ///// </summary>
    //public void ResetProgress()
    //{
    //    audioSource.Stop();
    //    audioSource.Play();
    //    slider.value = 0f;
    //}
}

// 强制立即更新布局 -无效
//LayoutRebuilder.ForceRebuildLayoutImmediate(twoName.GetComponent<RectTransform>());
//LayoutRebuilder.ForceRebuildLayoutImmediate(time.GetComponent<RectTransform>());
//AudioClip clip = Resources.Load<AudioClip>(CenterPanel.groupValues.NodeName);
//string path = "ToPutMusic/" + CenterPanel.groupValues.NodeName;
//Debug.Log(path);
//AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
//if (clip != null)
//{
//    Debug.Log("加载成功");
//    audioSource.clip = clip;
//}
//else
//{
//    Debug.Log("音频文件加载失败");
//}
/// <summary>
/// 将转化的audioclip导入audioSource
/// </summary>
/// //对start按钮进行点击监测-附在button组件
//public void ClickStart()
//{
//    //先判断是不是null状态
//}
//AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
//Debug.Log(audioClip);
//audioSource = GetComponent<AudioSource>();

//Debug.Log(time.text);
//将ToPutMusic的这个文件复制到resource文件夹中
//CopyToRes(CenterPanel.groupValues.NodeName);
//LoadClip();//加载音频文件
//slider.maxValue = audioSource.clip.length;//长度关联
// 启动定时更新 Slider 的协程 
//StartCoroutine(UpdateProgressCoroutine());
//if (count == 1)
//{
//    audioSource.volume = 0.05f;//很合适的音量~
//    audioSource.Play();
//}
//else
//{
//    audioSource.UnPause();
//}
////切换为暂停按钮
//count++;
//start.gameObject.SetActive(false);
//pause.gameObject.SetActive(true);
//// 结束拖动进度条时调用
//public void OnEndDrag()
//{
//    //isDragging = false;
//    // 确保在拖动结束时更新音频播放时间
//    audioSource.time = slider.value * CenterPanel.groupValues.Duration;
//}
//public void OnBeginDrag()//先点击一下再进行拖动
//{
//    if (!isDraggingProgress)
//    {
//        audioSource.Pause();
//        isDraggingProgress = true;
//    }
//    //audioSource.time = slider.value;
//    //Debug.Log(slider.value);
//}