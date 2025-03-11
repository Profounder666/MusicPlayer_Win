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
    private Transform content;//定义子子子对象content
    public GameObject columnPre;//定义每一栏的预设体
    private Transform whenNoColumn;
    private Text totalNum;
    
    void Start()
    {
        //获取content物体，以实例化的预设体为子物体
        content = transform.Find("Scroll View").Find("Viewport").Find("Content");
        whenNoColumn = transform.Find("WhenNoColumn");
        totalNum = transform.Find("ClassName").Find("Total").GetComponent<Text>();

        //每次打开软件自动检测一次本地文件夹
        DetectAndRead();
        //Debug.Log(audioFiles.Count);

    }
    private void Update()
    {
        //实时检测content对象之下有无子节点，若无则设置whennocolumn
        if (content.childCount == 0)
        {
            whenNoColumn.gameObject.SetActive(true);
        }
        else
        {
            whenNoColumn.gameObject.SetActive(false);
        }
    }

    //实现：检测特定位置的文件夹并返回提取到的特定信息
    //检测本地文件夹，提取信息并赋值
    private struct audioFilesInfo//定义一个结构体
    {
        public string title;//歌名
        public string singer;//歌手
        public string album;//专辑
        public float duration;//时长
        public string fileName;//文件名
    }
    static private Dictionary<string,audioFilesInfo> audioFiles;//定义一个字典，键是文件名，值是结构体
    public void DetectAndRead()
    {
        //步骤一：先检测目标文件夹
        Detect();
        //步骤二：将获取到的值赋值给预设体的文本
        SendToPre();
        //步骤三：给最上框的暂无total更新值
        ValueTotal();
        
    }
    private void Detect()
    {
        string localFolderPath = Path.Combine(Application.persistentDataPath, "ToPutMusic");
        //string localFolderPath = Path.Combine(Application.streamingAssetsPath, "ToPutMusic");
        // 检查 ToPutMusic 文件夹是否存在 
        if (!Directory.Exists(localFolderPath))
        {
            // 如果不存在，创建它 
            Directory.CreateDirectory(localFolderPath);
            Debug.Log("创建了 ToPutMusic 文件夹: " + localFolderPath);
        }
        string[] filePaths = Directory.GetFiles(localFolderPath, "*.mp3");//获取MP3文件

        audioFiles = new Dictionary<string, audioFilesInfo>();//因为每次都是在方法内重新初始化的，故不会保留
        //作用是将检索到的所有MP3文件路径存储在 filePaths 数组中
        //Debug.Log(filePaths.GetValue(0));//测试正确
        foreach (string path in filePaths)
        {
            try//以下的格式为taglib规范格式
            {
                using (TagLib.File file = TagLib.File.Create(path))
                {
                    //Debug.Log(file.TagTypes);//表示当前文件支持的标签类型
                    //Debug.Log(file.TagTypesOnDisk);//表示文件实际存储的标签类型-id3v2

                    audioFilesInfo fileInfo = new audioFilesInfo();
                    // 提取标题（歌名）
                    //fileInfo.title = file.GetTag(TagLib.TagTypes.Id3v2).MusicIpId;
                    fileInfo.title = file.Tag.Title?.ToString() ?? "未知";
                    //Debug.Log(fileInfo.title);
                    // 提取歌手 
                    //fileInfo.singer = file.Tag.AlbumArtists.Length > 0 ? file.Tag.AlbumArtists[0].ToString() : "未知";
                    //fileInfo.singer = file.Tag.AlbumArtists[0];//这个会报错
                    //fileInfo.singer = file.Tag.Artists[0]?.ToString() ?? "未知";//过时但是可行
                    fileInfo.singer = (file.Tag.AlbumArtists?.Length > 0 ?
                    file.Tag.AlbumArtists[0].ToString() :
                    file.Tag.Artists[0]?.ToString() ?? "未知");//综合考虑之下，照顾各个版本可用这个综合语句
                    //Debug.Log(fileInfo.singer);
                    // 提取专辑 
                    fileInfo.album = file.Tag.Album?.ToString() ?? "未知";
                    //Debug.Log(fileInfo.album);
                    // 提取时长（转换为秒）
                    fileInfo.duration = (float)file.Properties.Duration.TotalSeconds;
                    //Debug.Log(fileInfo.duration);
                    // 设置文件名 
                    fileInfo.fileName = Path.GetFileNameWithoutExtension(path);
                    //Debug.Log(fileInfo.fileName);
                    // 将信息添加到字典中 
                    audioFiles[fileInfo.fileName] = fileInfo;//没进行初始化就会报错
                    
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"无法读取文件 {Path.GetFileName(path)} 的元数据: {ex.Message}");
            }

        }
    }
    private void SendToPre()
    {
        //根据字典键值对的个数生成column单例,生成的同时就完成赋值
        //int i = 1;
        //进行赋值操作
        foreach (var file in audioFiles)
        {
            //string nodeName = $"{i.ToString()}-{file.Key}";
            //在赋值前，先确定是否已经有Column生成了,若没有才进行实例化
            if (!IsExisting(file.Key))
            {
                GameObject column = Instantiate(columnPre, transform.position, transform.rotation);
                column.transform.SetParent(content);
                column.transform.localScale = Vector3.one;//设置缩放为1
                column.transform.name = file.Key;
                //column.transform.name = i.ToString()+"-"+file.Key;//节点的命名格式：序号_文件名
                //column.transform.name = file.Key + "-" + i.ToString();

                Text songName = column.transform.Find("SongName").GetComponent<Text>();
                Text singerName = column.transform.Find("SingerName").GetComponent<Text>();
                Text album = column.transform.Find("Album").GetComponent<Text>();
                //Text order = column.transform.Find("Order").GetComponent<Text>();//序号
                //order.text = i.ToString();
                songName.text = file.Value.title;
                singerName.text = file.Value.singer;
                album.text = file.Value.album;
            }
            //i++;//这个要写在外面，不然第一个出现后后面就恒定为1了
        }

        //重新给order排序赋值
        int i = 1;
        foreach(Transform child in content)
        {
            Text order = child.Find("Order").GetComponent<Text>();
            order.text = i.ToString();
            i++;
        }
    }
    private bool IsExisting(string nodeName)//判断content节点下是否已经存在了nodeName这个节点了
    {
        if (content.childCount == 0||content==null)
        {
            return false;
        }
        //遍历所有子节点
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
            totalNum.text = "暂无";
            //这里利用刷新机制归零groupValues
            groupValues = new Group("", "", "", 0f);
        }
        else
        {
            totalNum.text = audioFiles.Count.ToString();
        }
    }

    //实现：点击栏传递一组值（）-每次单击栏就把字典键值对的值传过来
    public static Group groupValues = new Group("", "", "", 0f);//初始化
    public void GlobalAccess()//点击按钮时调用-这里默认了另外一个方法先调用//2
    {
        //Detect();//这一部分可能会损耗性能
        if (groupValues.NodeName != "") {
            //数据从字典中获取
            foreach(var file in audioFiles)
            {
                if (groupValues.NodeName == file.Key)
                {
                    //进行赋值操作
                    groupValues.SingerName = file.Value.singer;
                    groupValues.SongName = file.Value.title;
                    groupValues.Duration = file.Value.duration;
                }
            }
        }
        else
        {
            Debug.Log("并没有先获取到nodeName");
        }
        //Debug.Log(groupValues.SingerName+groupValues.SongName+groupValues.NodeName+groupValues.Duration);//这里用于测试
    }

}
//自定义Group类
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

//废弃代码
//Debug.Log(file.Value.title);
//if (songName != null)
//{
//    Debug.Log("存在");
//}
//else
//{//遍历content下每一个节点
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

//实现：给total赋值
//private Text total;
//private void ValueTotal()
//{
//    total = transform.Find("ClassName").Find("Total").GetComponent<Text>();
//    if (audioFiles.Count == 0||audioFiles==null)
//    {
//        total.text = "暂无";
//    }
//    else
//    {
//        total.text = audioFiles.Count.ToString();
//    }
//}

//string localFolderPath = Application.dataPath + "/StreamingAssets/ToputMusic";

//为避免因文件夹排序问题而产生的bug，这里刷新时就先将content的子节点全部消除释放-方法一
//if (content.childCount != 0)
//{
//    foreach(Transform child in content)
//    {
//        Destroy(child.gameObject);
//    }
//}