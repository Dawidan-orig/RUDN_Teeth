using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace MouthTrainer
{
    public class PosRotSaver : MonoBehaviour
    {
        public const string SAVE_DIRECTORY = "/SavedData/";
        public const string NAME_ENDING = ".sav";

        public Button saveButton;
        public Button loadButton;

        private void Start()
        {
            saveButton.onClick.AddListener(SaveData);
            loadButton.onClick.AddListener(RetriveData);
        }

        private struct SavedData
        {
            // Лучше сохранять локальные данные,
            // Чтобы можно было считывать объекты иерархии последовательно
            public Vector3 localPosition;
            public Quaternion localRotation;
            public string token;
        }

        public void SaveData()
        {
            SavedData data = new SavedData()
            {
                localPosition = transform.localPosition,
                localRotation = transform.localRotation,
                // Имена в сцене нужно делать уникальными, по этой причине.
                // Хотелось бы автоматизировать эту систему,
                // Но это выходит за рамки задачи. Потому лучше оставлю всё как есть.
                token = transform.name
            };

            var dir = Application.persistentDataPath + SAVE_DIRECTORY;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(dir + data.token + NAME_ENDING, json);
        }

        public void RetriveData()
        {
            var dir = Application.persistentDataPath + SAVE_DIRECTORY;
            string ID = transform.name;

            if (!File.Exists(dir + ID + NAME_ENDING))
                return;

            string json = File.ReadAllText(dir + ID + NAME_ENDING);
            SavedData data = JsonUtility.FromJson<SavedData>(json);

            transform.localPosition = data.localPosition;
            transform.localRotation = data.localRotation;
        }
    }
}