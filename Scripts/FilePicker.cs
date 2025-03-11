using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;
using System;
using UnityEngine.Networking;
using System.Collections;

public class FilePicker : MonoBehaviour
{//ָ���ṹ�����ڴ��еĲ��ַ�ʽΪ��˳�����У�Sequential�����ַ���Ϊ�Զ�ѡ��
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenFileName
    {
        public int structSize = 0;//��ʾ�ṹ�����ڴ��еĴ�С�����ֽ�Ϊ��λ��
        public IntPtr dlgOwner = IntPtr.Zero;//ָ���ֶ�dlgOwner����ʾӵ���ߴ��ڵľ����IntPtr.Zero��ʾû��ӵ���ߴ���
        public IntPtr instance = IntPtr.Zero;//instance����ʾʵ�������IntPtr.Zero��ʾû��ʵ�����
        public string filter = null;//filter������ָ���ļ������������ƶԻ�������ʾ���ļ�����
        public string customFilter = null;//customFilter������ָ���Զ�����ļ�������
        public int maxCustFilter = 0;//maxCustFilter����ʾ�Զ������������󳤶�
        public int filterIndex = 0;//ָ��Ĭ��ѡ�еĹ���������
        public string file = null;//���ڴ洢�û�ѡ�����ļ�·��
        public int maxFile = 0;//��ʾfile�ֶε��������
        public string fileTitle = null;//���ڴ洢�û�ѡ�����ļ���
        public int maxFileTitle = 0;//��ʾfileTitle�ֶε��������
        public string initialDir = null;//ָ���Ի����ʼ��ʾ��Ŀ¼·��
        public string title = null;//ָ���Ի���ı���
        public int flags = 0;//�������öԻ������Ϊ������ͨ��ʹ��λ��־���
        public short fileOffset = 0;//ָ���ļ��������·����ƫ����
        public short fileExtension = 0;//fileExtension��ָ���ļ���չ��������ļ�����ƫ����
        public string defExt = null;//ָ��Ĭ�ϵ��ļ���չ��
        public IntPtr custData = IntPtr.Zero;//���ڴ����Զ������ݡ�IntPtr.Zero��ʾû���Զ�������
        public IntPtr hook = IntPtr.Zero;//����ָ���ص������ľ����IntPtr.Zero��ʾû�лص�����
        public string templateName = null;//ָ���Ի���ģ�������
        public IntPtr reservedPtr = IntPtr.Zero;//������δ��ʹ�á�IntPtr.Zero��ʾδʹ��
        public int reservedInt = 0;//������δ��ʹ�á�0��ʾδʹ��
        public int flagsEx = 0;//����������չ���ܵı�־λ��0��ʾ����չ����
    }
    //����һ���ⲿ������ڵ㡣DllImportָʾ��������ָ���Ķ�̬���ӿ⣨DLL���е��뺯��
    //"Comdlg32.dll" ��Windowsϵͳ�е�һ��DLL���������õĶԻ�������
    //SetLastError=true ��ʾ����������ʧ��ʱ������ϵͳ�Ĵ����롣
    //CharSet=CharSet.Auto ��ʾ�ַ�����ƽ̨�Զ�ѡ��
    [DllImport("Comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
    //����һ���ⲿ��̬����GetOpenFileName�����ز���ֵ��
    //[In, Out] ��ʾ�ò�����������Ҳ�����������
    //OpenFileName ofn �Ǵ��ݸ������Ľṹ��ָ��
    public void ChooseAndCopyMP3()
    {
        OpenFileName openFileName = new OpenFileName();
        openFileName.structSize = Marshal.SizeOf(openFileName);//��structSize�ֶ�����Ϊ�ṹ�����ڴ��еĴ�С�����ֽ�Ϊ��λ����ʹ��Marshal.SizeOf������ȡ�ṹ���С
        openFileName.filter = "MP3(*.mp3)\0*.mp3\0All(*.*)\0*.*\0";//����filter�ֶ�ΪMP3�ļ��������ַ�����ʹ��\0��Ϊ�ָ������ָ���ͬ�Ĺ�����ѡ��
        //��һ�������������֣��ڶ�����Ϊ��չʽ����
        openFileName.file = new string(new char[256]);//��ʼ��file�ֶ�Ϊһ������Ϊ256�Ŀ��ַ�������
        openFileName.maxFile = openFileName.file.Length;//����maxFile�ֶ�Ϊfile����ĳ��ȣ�256��
        openFileName.fileTitle = new string(new char[64]);//�⼸�����б�Ҫ�ģ�����
        openFileName.maxFileTitle = openFileName.fileTitle.Length;
        openFileName.initialDir = Application.dataPath.Replace('/', '\\'); // Ĭ��·��
        //����initialDir�ֶ�ΪUnity��Application.dataPath ����ֵ������б���滻Ϊ��б������ӦWindows·����ʽ
        openFileName.title = "ѡ��mp3�ļ�";//����Ǵ������ϽǵĴ��ڱ���
        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        //ʹ��λ������϶����־λ��
        //0x00080000(OFN_NOCHANGEDIR)����ֹ�ı䵱ǰĿ¼��
        //0x00001000(OFN_PATHMUSTEXIST)��ȷ��·��������ڡ�
        //0x00000800(OFN_FILEMUSTEXIST)��ȷ��ѡ�����ļ�������ڡ�
        //0x00000008(OFN_HIDEREADONLY)������ֻ�����Ը�ѡ��
        
        if (GetOpenFileName(openFileName))//�����������true�����û��ɹ�ѡ�����ļ��������ȷ�ϰ�ť
        {
            string selectedFilePath = openFileName.file.TrimEnd('\0');//��ȡѡ�����ļ�·������ȥ��ĩβ�Ŀ���ֹ����\0��
            string destinationPath = Path.Combine(Application.persistentDataPath+"/ToPutMusic", Path.GetFileName(selectedFilePath));

            // ���Ŀ���ļ��Ƿ��Ѵ���
            if (File.Exists(destinationPath))
            {
                Debug.LogWarning($"�ļ��Ѵ�����Ŀ��·����{destinationPath}");
                return;
            }

            // �����ļ���PersistentDataPath
            File.Copy(selectedFilePath, destinationPath, true);
            Debug.Log($"�ļ��ѳɹ����Ƶ���{destinationPath}");
        }
    }
}

//ע�⣺�ⲿ�ִ���������̶������ʹ��

//�����ڴ˴����ʹ��Э�̱�����һ��active�������ϣ�����ʹ��prefab������
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

