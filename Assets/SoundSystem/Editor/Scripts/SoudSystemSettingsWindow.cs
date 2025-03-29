using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SoundSystem.Editor.Scripts
{
    public class SoudSystemSettingsWindow : EditorWindow
    {
        private string _enumFilePath = string.Empty;
        private string _enumFileContent;
        private List<string> _enumMembers = new List<string>();
        private string _editedEnumMembersText = "";
        private string _enumName = "SoundGroups"; // Имя вашего enum
        private string _scriptFileName = "SoundGroups"; // Имя файла скрипта enum

        [MenuItem("Tools/Sound system settings")]
        public static void ShowWindow() => GetWindow<SoudSystemSettingsWindow>("Sound System Settings");

        private void OnEnable()
        {
            FindEnumFile();
            if (!string.IsNullOrEmpty(_enumFilePath))
            {
                LoadEnumContent();
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Редактирование Enum", EditorStyles.boldLabel);

            if (string.IsNullOrEmpty(_enumFilePath))
            {
                EditorGUILayout.HelpBox($"Не удалось найти файл enum '{_scriptFileName}'. Убедитесь, что файл существует и называется правильно.", MessageType.Error);
                return;
            }

            if (_enumFileContent == null)
            {
                EditorGUILayout.HelpBox("Ошибка загрузки содержимого файла enum.", MessageType.Error);
                return;
            }

            GUILayout.Label($"Файл enum: {_enumFilePath}", EditorStyles.miniBoldLabel);

            EditorGUILayout.LabelField("Элементы Enum (построчно):");
            _editedEnumMembersText = EditorGUILayout.TextArea(_editedEnumMembersText, GUILayout.Height(150)); // TextArea для редактирования

            if (GUILayout.Button("Сохранить изменения в Enum"))
            {
                SaveChangesToEnumFile();
            }
        }

        private void FindEnumFile()
        {
            // Поиск файла скрипта enum в проекте
            string[] assets = AssetDatabase.FindAssets($"t:Script {_scriptFileName}");

            if (assets.Length > 0)
            {
                var filesPaths = assets.Select(asset => AssetDatabase.GUIDToAssetPath(asset));
                _enumFilePath = filesPaths.FirstOrDefault(path => Path.GetFileNameWithoutExtension(path) == _scriptFileName);

                if (!string.IsNullOrEmpty(_enumFilePath))
                {
                    Debug.Log($"Файл enum найден: {_enumFilePath}");
                    return;
                }
            }

            Debug.LogError($"Файл скрипта enum '{_scriptFileName}' не найден в проекте.");
        }

        private void LoadEnumContent()
        {
            try
            {
                _enumFileContent = File.ReadAllText(_enumFilePath);
                ParseEnumMembers();
                UpdateEditedText();
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка чтения файла enum: {e.Message}");
                _enumFileContent = null;
                _enumMembers.Clear();
                _editedEnumMembersText = "Ошибка чтения файла enum.";
            }
        }

        private void ParseEnumMembers()
        {
            _enumMembers.Clear();
            // Упрощенный парсинг: ищем начало и конец enum и извлекаем строки между ними
            string startMarker = $"enum {_enumName}";
            int startIndex = _enumFileContent.IndexOf(startMarker);
            if (startIndex == -1) return;

            int bodyStartIndex = _enumFileContent.IndexOf('{', startIndex);
            if (bodyStartIndex == -1) return;

            int bodyEndIndex = _enumFileContent.IndexOf('}', bodyStartIndex);
            if (bodyEndIndex == -1) return;

            string membersString = _enumFileContent.Substring(bodyStartIndex + 1, bodyEndIndex - bodyStartIndex - 1);
            string[] membersArray = membersString.Split(new char[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var member in membersArray)
            {
                _enumMembers.Add(member.Trim()); // Удаляем лишние пробелы
            }
        }

        private void UpdateEditedText()
        {
            _editedEnumMembersText = string.Join("\n", _enumMembers.ToArray());
        }

        private void SaveChangesToEnumFile()
        {
            try
            {
                // 1. Получаем отредактированные элементы из TextArea
                string[] editedMembersArray = _editedEnumMembersText.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> newEnumMembers = editedMembersArray.Select(m => m.Trim()).ToList(); // Trim и ToList

                // 2. Генерируем новое содержимое файла
                string newFileContent = GenerateNewEnumFileContent(newEnumMembers);

                // 3. Записываем в файл
                File.WriteAllText(_enumFilePath, newFileContent);

                // 4. Обновляем Unity Editor, чтобы он перекомпилировал скрипты
                AssetDatabase.Refresh();

                Debug.Log("Файл enum успешно обновлен!");
                LoadEnumContent(); // Перезагружаем содержимое после сохранения
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка сохранения файла enum: {e.Message}");
                EditorUtility.DisplayDialog("Ошибка", $"Не удалось сохранить файл enum: {e.Message}", "OK");
            }
        }

        private string GenerateNewEnumFileContent(List<string> newEnumMembers)
        {
            // Генерация нового содержимого файла на основе отредактированных элементов
            string indent = "    "; // Отступ для элементов enum

            int enumStartIndex = _enumFileContent.IndexOf($"enum {_enumName}");
            int enumEndIndex = _enumFileContent.IndexOf("}", enumStartIndex) + 2;

            string membersCode = "";
            for (int i = 0; i < newEnumMembers.Count; i++)
            {
                membersCode += indent + newEnumMembers[i] + ",\r\n";
            }

            string enumCode = $"enum {_enumName}\r\n{indent}{{\r\n{membersCode}{indent}}}\r\n";
            return _enumFileContent[..enumStartIndex] + enumCode + _enumFileContent[enumEndIndex..];
        }
    }
}
