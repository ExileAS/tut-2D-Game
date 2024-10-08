using System;
using System.IO;
using UnityEngine;

public class FileDataHandler {
    private string dirPath;
    private string fileNameStart;
    private string fileExtension;
    private string fullFileName;
    public static int fileIndex;
    private static readonly string codeWord = "Ajrpv3hPPoZqpgf7o8E6sSwAV68DfI5eewzRzZt9gDQ=";

    public FileDataHandler(string dirPath, string fileNameStart, string fileExtension)
    {
        this.dirPath = dirPath;
        this.fileNameStart = fileNameStart;
        this.fileExtension = fileExtension;
        fullFileName = fileNameStart + fileIndex + fileExtension;
        fileIndex = PlayerPrefs.GetInt("continue", 0);
        IncrementFileIndex();
    }

    public GameData LoadFromFile(string fileName, bool decrypt) {
        GameData gameData = null;
        string fullPath = Path.Combine(dirPath, fileName);

        if(File.Exists(fullPath)) {
            try
            {
                string loadedData;
                using(FileStream stream = new FileStream(fullPath, FileMode.Open)) {
                    using(StreamReader reader = new StreamReader(stream)) {
                        loadedData = reader.ReadToEnd();
                    }
                }
                if(decrypt) {
                    loadedData = AESUtil.Decrypt(loadedData);
                    loadedData = EncryptDecrypt(loadedData);
                }
                gameData = JsonUtility.FromJson<GameData>(loadedData);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        if(gameData != null) {
            fileIndex = PlayerPrefs.GetInt("continue", 0);
            IncrementFileIndex();
        }

        return gameData;
    }

    public void SaveToFile(GameData data, bool encrypt) {
        string fullPath = Path.Combine(dirPath, fullFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToSave = JsonUtility.ToJson(data, true);
            if(encrypt) {
                dataToSave = EncryptDecrypt(dataToSave);
                dataToSave = AESUtil.Encrypt(dataToSave);
            }
            using(FileStream stream = new FileStream(fullPath, FileMode.Create)) {
                using(StreamWriter writer = new StreamWriter(stream)) {
                    writer.Write(dataToSave);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        SaveToPlayerPrefs(data.sceneBuildIndex);
        IncrementFileIndex();
    }

    private void SaveToPlayerPrefs(int buildIndex) {
        PlayerPrefs.SetInt("continue", fileIndex);
        PlayerPrefs.SetString("TIME"+fileIndex, DateTime.Now.ToString());
        PlayerPrefs.SetInt("PROGRESS"+fileIndex, CheckPointManager.GetActivePercent());
        PlayerPrefs.SetInt("SCENE"+fileIndex, buildIndex);
        PlayerPrefs.Save();
    }

    private void IncrementFileIndex() {
        fileIndex = (fileIndex + 1) > 10 ? 1 : fileIndex + 1;
        fullFileName = fileNameStart + fileIndex + fileExtension;
    }

    private string EncryptDecrypt(string originalData) {
        string modifiedData = "";
        for(int i = 0; i < originalData.Length; i++) {
            modifiedData += (char) (originalData[i] ^ codeWord[i % codeWord.Length]);
        }
        return modifiedData;
    }
}