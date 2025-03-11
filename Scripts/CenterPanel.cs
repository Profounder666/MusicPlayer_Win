using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;

public class CenterPanel : MonoBehaviour
{
    private Transform content;//���������Ӷ���content
    public GameObject columnPre;//����ÿһ����Ԥ����
    private Transform whenNoColumn;
    private Text totalNum;
    
    void Start()
    {
        //��ȡcontent���壬��ʵ������Ԥ����Ϊ������
        content = transform.Find("Scroll View").Find("Viewport").Find("Content");
        whenNoColumn = transform.Find("WhenNoColumn");
        totalNum = transform.Find("ClassName").Find("Total").GetComponent<Text>();

        //ÿ�δ�����Զ����һ�α����ļ���
        DetectAndRead();
        //Debug.Log(audioFiles.Count);

    }
    private void Update()
    {
        //ʵʱ���content����֮�������ӽڵ㣬����������whennocolumn
        if (content.childCount == 0)
        {
            whenNoColumn.gameObject.SetActive(true);
        }
        else
        {
            whenNoColumn.gameObject.SetActive(false);
        }
    }

    //ʵ�֣�����ض�λ�õ��ļ��в�������ȡ�����ض���Ϣ
    //��Ȿ���ļ��У���ȡ��Ϣ����ֵ
    private struct audioFilesInfo//����һ���ṹ��
    {
        public string title;//����
        public string singer;//����
        public string album;//ר��
        public float duration;//ʱ��
        public string fileName;//�ļ���
    }
    static private Dictionary<string,audioFilesInfo> audioFiles;//����һ���ֵ䣬�����ļ�����ֵ�ǽṹ��
    public void DetectAndRead()
    {
        //����һ���ȼ��Ŀ���ļ���
        Detect();
        //�����������ȡ����ֵ��ֵ��Ԥ������ı�
        SendToPre();
        //�������������Ͽ������total����ֵ
        ValueTotal();
        
    }
    private void Detect()
    {
        string localFolderPath = Path.Combine(Application.persistentDataPath, "ToPutMusic");
        //string localFolderPath = Path.Combine(Application.streamingAssetsPath, "ToPutMusic");
        // ��� ToPutMusic �ļ����Ƿ���� 
        if (!Directory.Exists(localFolderPath))
        {
            // ��������ڣ������� 
            Directory.CreateDirectory(localFolderPath);
            Debug.Log("������ ToPutMusic �ļ���: " + localFolderPath);
        }
        string[] filePaths = Directory.GetFiles(localFolderPath, "*.mp3");//��ȡMP3�ļ�

        audioFiles = new Dictionary<string, audioFilesInfo>();//��Ϊÿ�ζ����ڷ��������³�ʼ���ģ��ʲ��ᱣ��
        //�����ǽ�������������MP3�ļ�·���洢�� filePaths ������
        //Debug.Log(filePaths.GetValue(0));//������ȷ
        foreach (string path in filePaths)
        {
            try//���µĸ�ʽΪtaglib�淶��ʽ
            {
                using (TagLib.File file = TagLib.File.Create(path))
                {
                    //Debug.Log(file.TagTypes);//��ʾ��ǰ�ļ�֧�ֵı�ǩ����
                    //Debug.Log(file.TagTypesOnDisk);//��ʾ�ļ�ʵ�ʴ洢�ı�ǩ����-id3v2

                    audioFilesInfo fileInfo = new audioFilesInfo();
                    // ��ȡ���⣨������
                    //fileInfo.title = file.GetTag(TagLib.TagTypes.Id3v2).MusicIpId;
                    fileInfo.title = file.Tag.Title?.ToString() ?? "δ֪";
                    //Debug.Log(fileInfo.title);
                    // ��ȡ���� 
                    //fileInfo.singer = file.Tag.AlbumArtists.Length > 0 ? file.Tag.AlbumArtists[0].ToString() : "δ֪";
                    //fileInfo.singer = file.Tag.AlbumArtists[0];//����ᱨ��
                    //fileInfo.singer = file.Tag.Artists[0]?.ToString() ?? "δ֪";//��ʱ���ǿ���
                    fileInfo.singer = (file.Tag.AlbumArtists?.Length > 0 ?
                    file.Tag.AlbumArtists[0].ToString() :
                    file.Tag.Artists[0]?.ToString() ?? "δ֪");//�ۺϿ���֮�£��չ˸����汾��������ۺ����
                    //Debug.Log(fileInfo.singer);
                    // ��ȡר�� 
                    fileInfo.album = file.Tag.Album?.ToString() ?? "δ֪";
                    //Debug.Log(fileInfo.album);
                    // ��ȡʱ����ת��Ϊ�룩
                    fileInfo.duration = (float)file.Properties.Duration.TotalSeconds;
                    //Debug.Log(fileInfo.duration);
                    // �����ļ��� 
                    fileInfo.fileName = Path.GetFileNameWithoutExtension(path);
                    //Debug.Log(fileInfo.fileName);
                    // ����Ϣ��ӵ��ֵ��� 
                    audioFiles[fileInfo.fileName] = fileInfo;//û���г�ʼ���ͻᱨ��
                    
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"�޷���ȡ�ļ� {Path.GetFileName(path)} ��Ԫ����: {ex.Message}");
            }

        }
    }
    private void SendToPre()
    {
        //�����ֵ��ֵ�Եĸ�������column����,���ɵ�ͬʱ����ɸ�ֵ
        //int i = 1;
        //���и�ֵ����
        foreach (var file in audioFiles)
        {
            //string nodeName = $"{i.ToString()}-{file.Key}";
            //�ڸ�ֵǰ����ȷ���Ƿ��Ѿ���Column������,��û�вŽ���ʵ����
            if (!IsExisting(file.Key))
            {
                GameObject column = Instantiate(columnPre, transform.position, transform.rotation);
                column.transform.SetParent(content);
                column.transform.localScale = Vector3.one;//��������Ϊ1
                column.transform.name = file.Key;
                //column.transform.name = i.ToString()+"-"+file.Key;//�ڵ��������ʽ�����_�ļ���
                //column.transform.name = file.Key + "-" + i.ToString();

                Text songName = column.transform.Find("SongName").GetComponent<Text>();
                Text singerName = column.transform.Find("SingerName").GetComponent<Text>();
                Text album = column.transform.Find("Album").GetComponent<Text>();
                //Text order = column.transform.Find("Order").GetComponent<Text>();//���
                //order.text = i.ToString();
                songName.text = file.Value.title;
                singerName.text = file.Value.singer;
                album.text = file.Value.album;
            }
            //i++;//���Ҫд�����棬��Ȼ��һ�����ֺ����ͺ㶨Ϊ1��
        }

        //���¸�order����ֵ
        int i = 1;
        foreach(Transform child in content)
        {
            Text order = child.Find("Order").GetComponent<Text>();
            order.text = i.ToString();
            i++;
        }
    }
    private bool IsExisting(string nodeName)//�ж�content�ڵ����Ƿ��Ѿ�������nodeName����ڵ���
    {
        if (content.childCount == 0||content==null)
        {
            return false;
        }
        //���������ӽڵ�
        for(int i=0;i<content.childCount ;i++)
        {
            if (content.GetChild(i).name == nodeName)
            {
                return true;
            }
        }
        return false;
    }
    private void ValueTotal()
    {
        if (audioFiles.Count == 0 || audioFiles == null)
        {
            totalNum.text = "����";
            //��������ˢ�»��ƹ���groupValues
            groupValues = new Group("", "", "", 0f);
        }
        else
        {
            totalNum.text = audioFiles.Count.ToString();
        }
    }

    //ʵ�֣����������һ��ֵ����-ÿ�ε������Ͱ��ֵ��ֵ�Ե�ֵ������
    public static Group groupValues = new Group("", "", "", 0f);//��ʼ��
    public void GlobalAccess()//�����ťʱ����-����Ĭ��������һ�������ȵ���//2
    {
        //Detect();//��һ���ֿ��ܻ��������
        if (groupValues.NodeName != "") {
            //���ݴ��ֵ��л�ȡ
            foreach(var file in audioFiles)
            {
                if (groupValues.NodeName == file.Key)
                {
                    //���и�ֵ����
                    groupValues.SingerName = file.Value.singer;
                    groupValues.SongName = file.Value.title;
                    groupValues.Duration = file.Value.duration;
                }
            }
        }
        else
        {
            Debug.Log("��û���Ȼ�ȡ��nodeName");
        }
        //Debug.Log(groupValues.SingerName+groupValues.SongName+groupValues.NodeName+groupValues.Duration);//�������ڲ���
    }

}
//�Զ���Group��
public class Group
{
    public string SingerName { get; set; }
    public string SongName { get; set; }
    public float Duration { get; set; }
    public string NodeName { get; set; }
    public Group(string nodeName,string singerName, string songName, float duration)
    {
        NodeName = nodeName;
        SingerName = singerName;
        SongName = songName;
        Duration = duration;
    }
}

//��������
//Debug.Log(file.Value.title);
//if (songName != null)
//{
//    Debug.Log("����");
//}
//else
//{//����content��ÿһ���ڵ�
//foreach(Transform node in content)
//{
//    if (node.name == nodeName)
//    {
//        result = true;
//    }
//    else
//    {
//        result = false;
//    }
//}
//return result;
//}

//ʵ�֣���total��ֵ
//private Text total;
//private void ValueTotal()
//{
//    total = transform.Find("ClassName").Find("Total").GetComponent<Text>();
//    if (audioFiles.Count == 0||audioFiles==null)
//    {
//        total.text = "����";
//    }
//    else
//    {
//        total.text = audioFiles.Count.ToString();
//    }
//}

//string localFolderPath = Application.dataPath + "/StreamingAssets/ToputMusic";

//Ϊ�������ļ������������������bug������ˢ��ʱ���Ƚ�content���ӽڵ�ȫ�������ͷ�-����һ
//if (content.childCount != 0)
//{
//    foreach(Transform child in content)
//    {
//        Destroy(child.gameObject);
//    }
//}