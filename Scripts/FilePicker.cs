using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;
using System;
using UnityEngine.Networking;
using System.Collections;

public class FilePicker : MonoBehaviour
{//指定结构体在内存中的布局方式为按顺序排列（Sequential），字符集为自动选择
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenFileName
    {
        public int structSize = 0;//表示结构体在内存中的大小（以字节为单位）
        public IntPtr dlgOwner = IntPtr.Zero;//指针字段dlgOwner，表示拥有者窗口的句柄。IntPtr.Zero表示没有拥有者窗口
        public IntPtr instance = IntPtr.Zero;//instance，表示实例句柄。IntPtr.Zero表示没有实例句柄
        public string filter = null;//filter，用于指定文件过滤器，控制对话框中显示的文件类型
        public string customFilter = null;//customFilter，用于指定自定义的文件过滤器
        public int maxCustFilter = 0;//maxCustFilter，表示自定义过滤器的最大长度
        public int filterIndex = 0;//指定默认选中的过滤器索引
        public string file = null;//用于存储用户选定的文件路径
        public int maxFile = 0;//表示file字段的最大容量
        public string fileTitle = null;//用于存储用户选定的文件名
        public int maxFileTitle = 0;//表示fileTitle字段的最大容量
        public string initialDir = null;//指定对话框初始显示的目录路径
        public string title = null;//指定对话框的标题
        public int flags = 0;//用于配置对话框的行为特征。通常使用位标志组合
        public short fileOffset = 0;//指定文件名相对于路径的偏移量
        public short fileExtension = 0;//fileExtension，指定文件扩展名相对于文件名的偏移量
        public string defExt = null;//指定默认的文件扩展名
        public IntPtr custData = IntPtr.Zero;//用于传递自定义数据。IntPtr.Zero表示没有自定义数据
        public IntPtr hook = IntPtr.Zero;//用于指定回调函数的句柄。IntPtr.Zero表示没有回调函数
        public string templateName = null;//指定对话框模板的名称
        public IntPtr reservedPtr = IntPtr.Zero;//保留供未来使用。IntPtr.Zero表示未使用
        public int reservedInt = 0;//保留供未来使用。0表示未使用
        public int flagsEx = 0;//用于配置扩展功能的标志位。0表示无扩展功能
    }
    //声明一个外部函数入口点。DllImport指示编译器从指定的动态链接库（DLL）中导入函数
    //"Comdlg32.dll" 是Windows系统中的一个DLL，包含常用的对话框函数。
    //SetLastError=true 表示当函数调用失败时会设置系统的错误码。
    //CharSet=CharSet.Auto 表示字符集随平台自动选择。
    [DllImport("Comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
    //声明一个外部静态方法GetOpenFileName，返回布尔值。
    //[In, Out] 表示该参数既是输入也是输出参数。
    //OpenFileName ofn 是传递给函数的结构体指针
    public void ChooseAndCopyMP3()
    {
        OpenFileName openFileName = new OpenFileName();
        openFileName.structSize = Marshal.SizeOf(openFileName);//将structSize字段设置为结构体在内存中的大小（以字节为单位）。使用Marshal.SizeOf方法获取结构体大小
        openFileName.filter = "MP3(*.mp3)\0*.mp3\0All(*.*)\0*.*\0";//设置filter字段为MP3文件过滤器字符串。使用\0作为分隔符来分隔不同的过滤器选项
        //第一部分是描述部分，第二部分为扩展式部分
        openFileName.file = new string(new char[256]);//初始化file字段为一个长度为256的空字符串数组
        openFileName.maxFile = openFileName.file.Length;//设置maxFile字段为file数组的长度（256）
        openFileName.fileTitle = new string(new char[64]);//这几行是有必要的！！！
        openFileName.maxFileTitle = openFileName.fileTitle.Length;
        openFileName.initialDir = Application.dataPath.Replace('/', '\\'); // 默认路径
        //设置initialDir字段为Unity的Application.dataPath 属性值，并将斜杠替换为反斜杠以适应Windows路径格式
        openFileName.title = "选择mp3文件";//这个是窗口左上角的窗口标题
        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        //使用位运算组合多个标志位：
        //0x00080000(OFN_NOCHANGEDIR)：防止改变当前目录。
        //0x00001000(OFN_PATHMUSTEXIST)：确保路径必须存在。
        //0x00000800(OFN_FILEMUSTEXIST)：确保选定的文件必须存在。
        //0x00000008(OFN_HIDEREADONLY)：隐藏只读属性复选框
        
        if (GetOpenFileName(openFileName))//如果函数返回true，则用户成功选择了文件并点击了确认按钮
        {
            string selectedFilePath = openFileName.file.TrimEnd('\0');//获取选定的文件路径，并去除末尾的空终止符（\0）
            string destinationPath = Path.Combine(Application.persistentDataPath+"/ToPutMusic", Path.GetFileName(selectedFilePath));

            // 检查目标文件是否已存在
            if (File.Exists(destinationPath))
            {
                Debug.LogWarning($"文件已存在于目标路径：{destinationPath}");
                return;
            }

            // 复制文件到PersistentDataPath
            File.Copy(selectedFilePath, destinationPath, true);
            Debug.Log($"文件已成功复制到：{destinationPath}");
        }
    }
}

//注意：这部分代码可视作固定框架来使用

//下面在此处解决使用协程必须在一个active的物体上，不得使用prefab的问题
public class GameCoroutine : MonoBehaviour
{
    private static GameCoroutine _instance;
    public static GameCoroutine Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("GameCoroutine");
                _instance = obj.AddComponent<GameCoroutine>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    public void StartCoroutines(IEnumerator routine)
    {
        StartCoroutine(routine);
    }
    public void StopCoroutines(IEnumerator routine)
    {
        StopCoroutine(routine);
    }
    
}



//public class GameCoroutine : MonoBehaviour
//{
//    private static GameCoroutine _instance;
//    public static GameCoroutine Instance
//    {
//        get
//        {
//            if (_instance == null)
//            {
//                GameObject obj = new GameObject("GameCoroutine");
//                _instance = obj.AddComponent<GameCoroutine>();
//                DontDestroyOnLoad(obj);
//            }
//            return _instance;
//        }
//    }
//    public void StartCoroutines(IEnumerator routine)
//    {
//        StartCoroutine(routine);
//    }
//    public void StopCoroutines(IEnumerator routine)
//    {
//        StopCoroutine(routine);
//    }

