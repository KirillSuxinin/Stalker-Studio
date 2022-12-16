using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Stalker_Studio.Common
{
    /// <summary>
    /// Класс работы с текстовым файлом 
    /// Считается сериализуемым,
    /// Результат сериализации: Text
    /// Результат десериализации: Text
    /// </summary>
    public class TextFileModel : SerializableFileModel
    {
        protected string _text = "";
        protected Encoding _encoding = Encoding.Unicode;

        public TextFileModel() : base() { }
        public TextFileModel(FileInfo file, Encoding encoding = null) : base(file)
        {
            // автосериализация не нужна, так как сериализовать нечего
            // но поддержку автосериализации реализовывать обязательно, для наследников

            if(encoding != null)
                _encoding = encoding;
        }
        public TextFileModel(string text, Encoding encoding = null) : base()
        {
            // автосериализация не нужна, так как сериализовать нечего
            // но поддержку автосериализации реализовывать обязательно

            _text = text;
            if (encoding != null)
                _encoding = encoding;
        }

        /// <summary>
        /// Текст
        /// </summary>
        public string Text { 
            get {
                UpdateSerialization();// обязательно для поддержки автосериализации
                return _text; 
            } 
            set {
                _text = value;
                OnPropertyChanged();

                // Поддержка автосериализации
                // Поменялся текст, значит надо обновить другие данные, которые получаем с помощью десериализации из этого текста
                SerializationState = SerializationState.NeedDeserialize;
            } 
        }
        /// <summary>
        /// Кодировка
        /// </summary>
        public Encoding Encoding
        {
            get { return _encoding; }
            set
            {
                _encoding = value;
                OnPropertyChanged();
            }
        }

        public override void Load()
        {
            StreamReader streamReader = new StreamReader(_fullName, _encoding);
            _text = streamReader.ReadToEnd();
            streamReader.Close();
        }
        public override void Save()
        {
            StreamWriter streamWriter = new StreamWriter(_fullName, false, _encoding);
            streamWriter.Write(_text);
            streamWriter.Close();
        }


        #region Поддержка автоматического управления сериализацией

        protected override void OnSetSerializationState()
        {
            // Добавляем действия при изменении состояния сериализации, в зависимости от особенностей класса
            if (SerializationState == SerializationState.NeedSerialize)
                // Так как очередная сериализация должна быть выполнена при обращении к свойству Text, 
                //  вызываем OnPropertyChanged("Text") что бы справоцировать 
                //  это обращение из элементов интерфейса (и не только), к которым привязано свойство Text
                OnPropertyChanged("Text");
        }
        protected override string GetSerializedString()
        {
            return _text;// возвращаем текст файла
        }
        protected override void OnAutoSerialized(string serializedString)
        {
            _text = serializedString;   //
        }

        #endregion


        protected override string ConvertToString()
        {
            return _text;// затычка
        }
        public override void Deserialize(string serializedString = "") 
        {
            _text = serializedString;// затычка
        }
    }
}
