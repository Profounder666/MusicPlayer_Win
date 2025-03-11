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
    private Transform start;//�ȶ��弸���ӽڵ�
    private Transform pause;
    private Transform left;
    private Transform right;
    private Transform volume;//�����ҽ�volume��Ϊ����Դ
    private Transform mute;
    private Transform mode;

    static private Text twoName;
    static private Text time;

    //�����벥����Ƶ�ļ����
    static public AudioSource audioSource;
    static public CustomSlider slider;
    static public AudioClip audioClip;
    public CustomSlider customSlider;
    static public Transform volumeSlider;
    static public Text modeText;

    // Start is called before the first frame update
    void Start()
    {
        //�ӽڵ��ʼ��
        start = transform.Find("Start");
        pause = transform.Find("Pause");
        pause.gameObject.SetActive(false);//������Ϊfalse
        volume = transform.Find("Volume");
        mute = transform.Find("Mute");
        //mute.gameObject.SetActive(false);
        left = transform.Find("Left");
        right = transform.Find("Right");
        mode = transform.Find("Mode");
        //�����ʼֵ
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
        //TotalState();//�Ƿ���Ի��һ��ÿ������أ�
        //transform.gameObject.SetActive(true);
        OnProgressChanged();
    }

    
    //ʵ�֣�ʱ��ת�����������ǿ������ַ�������
    private string DurationTransform(float origin)
    {
        string result = "";
        int min = 0;//�������
        int second = 0;//������
        int current = (int)Math.Ceiling(origin);
        while (current >= 60)
        {
            current -= 60;
            min++;
        }
        second = current;
        //�������ת��-Ĭ����Ϊ����min������2λ��(����ֺ��붼��2λ����ʾ)
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

    //ʵ�֣��¿������ۺϵ�һ������-��һ�׸��״̬��ʽ����
    public void TotalState()//������ֵ�Ѿ�����������//3
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
        // ָ����Ƶ�ļ�������·�� 
        fileName += ".mp3";
        string filePath = Application.dataPath + "/ToPutMusic/" + fileName;
        Debug.Log(filePath);
        // ����ļ��Ƿ���� 
        if (File.Exists(filePath))
        {
            // ���ļ����Ƶ� Resources �ļ��� 
            string resourcesPath = Path.Combine(Application.dataPath, "Resources");
            string destinationPath = Path.Combine(resourcesPath,fileName);

            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
                Debug.Log(resourcesPath);
            }

            File.WriteAllBytes(destinationPath, File.ReadAllBytes(filePath));
        }
    }//�������̫���ˣ�����Ҫ�ܾ�
    //����resources��Ļ����ļ�
    private void ClearRes()
    {
        // ��ȡ Resources �ļ���·�� 
        string resourcesPath = Path.Combine(Application.dataPath, "Resources");

        // ������ɾ�� Resources �ļ����е������ļ� 
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
        string url = Path.Combine(Application.persistentDataPath,"ToPutMusic",CenterPanel.groupValues.NodeName+".mp3");// �洢·��
        byte[] bytes = GetMP3Bytes(url); // ��ȡMP3�ļ����ֽ�����

        File.WriteAllBytes(url, bytes);  // д���ļ�
        //Debug.Log(CenterPanel.groupValues.NodeName);//������
        //Debug.Log(transform.gameObject.activeSelf);
        if (transform.gameObject.activeSelf)
        {
            //Debug.Log("�ѵ���Э��");
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
            Debug.Log("�ļ���תΪ��������");
            return File.ReadAllBytes(path);
        }
        else
        {
            Debug.Log("�ļ�����תΪ��������");
            return new byte[0];
        }
    }
    IEnumerator BinaryToClip1(string filePath)
    {
        // �������ļ�·��ת��Ϊ URI ��ʽ
        string uri = "file://" + filePath;
        Debug.Log(uri);
        // ʹ�� UnityWebRequest ������Ƶ�ļ���ת��Ϊ AudioClip
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.MPEG))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(uwr.error);
            }
            else
            {
                // ��ȡ AudioClip
                AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                if (clip != null)
                {
                    Debug.Log("�ɹ���ȡclip��");
                    //AudioSource audioSource = GetComponent<AudioSource>();
                    //if (audioSource != null)
                    //{
                    //    audioSource.clip = clip;
                    //    //audioSource.Play();
                    //    Debug.Log("clip��ȡ�ɹ�");
                    //}
                    //else
                    //{
                    //    Debug.LogError("AudioSource ���δ�ҵ���");
                    //}
                }
                else
                {
                    Debug.LogError("�޷���ȡ AudioClip��");
                }
            }
        }
    }
    IEnumerator BinaryToClip(string filePath, byte[] bytes) {
        try
        {//·��һ��Ҫ���Ǻ�׺��չ��������
            // ʹ�� MemoryStream �� Mp3FileReader ���� MP3 ����
            using (MemoryStream mp3Stream = new MemoryStream(bytes))
            using (Mp3FileReader mp3Reader = new Mp3FileReader(mp3Stream))
            //using (var mp3Reader = new MediaFoundationReader(filePath))
            using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(mp3Reader))
            {
                // ��ȡ��Ƶ��Ϣ
                int sampleRate = pcmStream.WaveFormat.SampleRate;
                int channels = pcmStream.WaveFormat.Channels;
                long sampleCount = pcmStream.Length / (channels * (pcmStream.WaveFormat.BitsPerSample / 8));

                // ���� AudioClip-clip�ļ���ΪConvertedAudio
                audioClip = AudioClip.Create(CenterPanel.groupValues.SongName, (int)sampleCount, channels, sampleRate, false);

                // ��ȡ PCM ���ݲ����õ� AudioClip
                float[] samples = new float[sampleCount * channels];
                byte[] buffer = new byte[pcmStream.Length];
                int bytesRead = pcmStream.Read(buffer, 0, buffer.Length);
                if (bytesRead != buffer.Length)
                {
                    Debug.LogError("δ��ȡȫ�����ݣ�");//�����Ѿ���ȡȫ������
                    yield break;
                }

                for (int i = 0; i < bytesRead / 2; i++)
                {
                    short sample = BitConverter.ToInt16(buffer, i * 2);
                    samples[i] = sample / 32768.0f;
                }
                audioClip.SetData(samples, 0);

                //Debug.Log("clip�����ɹ�");
                // ������Ƶ
                audioSource = GetComponent<AudioSource>();
                //AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource != null || this.gameObject.activeInHierarchy)
                {
                    audioSource.clip = audioClip;//��Ϊ����һ����ʱ�ļ��������ڿ��ӻ�����п��ܲ�����ʾ
                    Debug.Log("��Ƶ����ɹ�!"+audioSource.clip.name);
                    //AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
                    //audioSource.Play();//�ᱨ��-����
                    //Debug.Log(audioSource.isPlaying);
                }
                else
                {
                    Debug.Log("AudioSource ���δ�ҵ���");
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log($"��Ƶת��ʧ��: {ex.Message}");
        }
        yield return null;
    }
    ///<summary>
    ///�㲥�Ű�ť���в���
    ///</summary>
    public void OnClickPlay()
    {
        //�ж�clip�Ƿ��Ѿ�����source
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
            //�Ȼ�ȡ�û�ԭ����������С
            if (audioSource.volume!=0)//�Ǿ���״̬
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
    /// ���� Slider ֵ�仯�¼���������Ƶ����ʱ�� 
    /// </summary>
    /// <param name="value">Slider �ĵ�ǰֵ</param>
    static public bool isDraggingProgress = false;//�϶�״̬���
    private void OnProgressChanged()
    {
        if (audioSource.isPlaying&&!isDraggingProgress)
        {
            // �� Slider ��ֵ����Ϊ��Ƶ�ĵ�ǰʱ�� 
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
    ///���������Ĵ�С
    ///</summary>
    public void OnClickVolume()
    {
        CustomSlider slider = volumeSlider.GetComponent<CustomSlider>();
        slider.maxValue = 0.1f;
        slider.value = audioSource.volume;
        //Debug.Log(volumeSlider.gameObject.activeSelf);
        if (volumeSlider.gameObject.activeSelf==false)//�������
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
        //modeText.text = "ѭ������(�������ŷ�ʽ�����ڴ�)";
    }
    public void OnClickClose()
    {
        Application.Quit();
        Debug.Log("�˳��ɹ�");
        // �� Windows �Ͽ���ʹ�����´���ǿ���˳� 
//#if UNITY_STANDALONE_WIN
//        System.Diagnostics.Process.GetCurrentProcess().Kill();
//#endif
    }
    
    //IEnumerator DoVioceClip(string url)//����һ����������ΪIEnumerator��Э�̷���������һ���ַ�������VoiceUrl
    //{
    //    //����һ��UnityWebRequest����uwr�����ڴ�ָ����URL����MP3��ʽ����Ƶ������AudioType.MPEGָ�����ص���MP3��ʽ����Ƶ
    //    UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
    //    yield return uwr.SendWebRequest();
    //    //if (uwr.isNetworkError)//��ʱ
    //    if(uwr.result==UnityWebRequest.Result.ConnectionError)
    //        Debug.LogError(uwr.error);
    //    else
    //    {
    //        AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
    //        Debug.Log("ת���ɹ�");
    //        //AS.clip = clip;
    //        //AS.Play();
    //    }
    //}
    //private void LoadToSource(string filePath)
    //{
    //    if (File.Exists(filePath))
    //    {
    //        // ʹ���ļ�·��������Ƶ���� 
    //        AudioClip clip;
    //        clip = AudioClip.LoadAudioData(filePath, AudioType.MPEG);

    //        if (clip != null)
    //        {
    //            // �� AudioClip ���䵽 AudioSource ��� 
    //            GetComponent<AudioSource>().clip = clip;
    //            Debug.Log("�ɹ�����: " + filePath);
    //        }
    //        else
    //        {
    //            Debug.LogError("�޷����� AudioClip: " + filePath);
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("��Ƶ�ļ�������: " + filePath);
    //    }
    //}
    /// <summary>
    /// ��ʱ���� Slider ��Э�̷���
    /// </summary>
    /// <returns></returns>
    //private IEnumerator UpdateProgressCoroutine()
    //{
    //    while (true)
    //    {
    //        // ��ȡ��ǰ����ʱ�� 
    //        float currentTime = audioSource.time;
    //        // ���� Slider ��ֵ 
    //        slider.value = currentTime;
    //        // �ȴ�һ���Ӻ��ٴ�ִ�� 
    //        yield return new WaitForSeconds(1f);
    //    }
    //}

    ///// <summary>
    ///// ���� Slider ����Ƶ���� 
    ///// </summary>
    //public void ResetProgress()
    //{
    //    audioSource.Stop();
    //    audioSource.Play();
    //    slider.value = 0f;
    //}
}

// ǿ���������²��� -��Ч
//LayoutRebuilder.ForceRebuildLayoutImmediate(twoName.GetComponent<RectTransform>());
//LayoutRebuilder.ForceRebuildLayoutImmediate(time.GetComponent<RectTransform>());
//AudioClip clip = Resources.Load<AudioClip>(CenterPanel.groupValues.NodeName);
//string path = "ToPutMusic/" + CenterPanel.groupValues.NodeName;
//Debug.Log(path);
//AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
//if (clip != null)
//{
//    Debug.Log("���سɹ�");
//    audioSource.clip = clip;
//}
//else
//{
//    Debug.Log("��Ƶ�ļ�����ʧ��");
//}
/// <summary>
/// ��ת����audioclip����audioSource
/// </summary>
/// //��start��ť���е�����-����button���
//public void ClickStart()
//{
//    //���ж��ǲ���null״̬
//}
//AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
//Debug.Log(audioClip);
//audioSource = GetComponent<AudioSource>();

//Debug.Log(time.text);
//��ToPutMusic������ļ����Ƶ�resource�ļ�����
//CopyToRes(CenterPanel.groupValues.NodeName);
//LoadClip();//������Ƶ�ļ�
//slider.maxValue = audioSource.clip.length;//���ȹ���
// ������ʱ���� Slider ��Э�� 
//StartCoroutine(UpdateProgressCoroutine());
//if (count == 1)
//{
//    audioSource.volume = 0.05f;//�ܺ��ʵ�����~
//    audioSource.Play();
//}
//else
//{
//    audioSource.UnPause();
//}
////�л�Ϊ��ͣ��ť
//count++;
//start.gameObject.SetActive(false);
//pause.gameObject.SetActive(true);
//// �����϶�������ʱ����
//public void OnEndDrag()
//{
//    //isDragging = false;
//    // ȷ�����϶�����ʱ������Ƶ����ʱ��
//    audioSource.time = slider.value * CenterPanel.groupValues.Duration;
//}
//public void OnBeginDrag()//�ȵ��һ���ٽ����϶�
//{
//    if (!isDraggingProgress)
//    {
//        audioSource.Pause();
//        isDraggingProgress = true;
//    }
//    //audioSource.time = slider.value;
//    //Debug.Log(slider.value);
//}