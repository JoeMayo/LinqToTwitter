/**
 * JsonData.cs
 *   Generic jsonType to hold JSON data (objects, arrays, and so on). This is
 *   the default jsonType returned by JsonMapper.ToObject().
 *
 * The authors disclaim copyright to this source code. For more details, see
 * the COPYING file included with this distribution.
 **/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace LitJson
{
    public class JsonData : IJsonWrapper, IEquatable<JsonData>
    {
        internal IDictionary<string, JsonData> InstObject;

        private IList<JsonData>               instArray;
        private bool                          instBoolean;
        private double                        instDouble;
        private int                           instInt;
        private long                          instLong;
        private string                        instString;
        private string                        json;
        private JsonType                      type;
        decimal instDecimal;
        ulong instUlong;

        // Used to implement the IOrderedDictionary interface
        private IList<KeyValuePair<string, JsonData>> objectList;

        public int Count {
            get { return EnsureCollection ().Count; }
        }

        public bool IsArray {
            get { return type == JsonType.Array; }
        }

        public bool IsBoolean {
            get { return type == JsonType.Boolean; }
        }

        public bool IsDouble {
            get { return type == JsonType.Double; }
        }

        public bool IsInt {
            get { return type == JsonType.Int; }
        }

        public bool IsLong {
            get { return type == JsonType.Long; }
        }

        public bool IsObject {
            get { return type == JsonType.Object; }
        }

        public bool IsString {
            get { return type == JsonType.String; }
        }

        public bool IsDecimal
        {
            get { return type == JsonType.Decimal; }
        }

        public bool IsULong
        {
            get { return type == JsonType.ULong; }
        }

        int ICollection.Count {
            get {
                return Count;
            }
        }

        bool ICollection.IsSynchronized {
            get {
                return EnsureCollection ().IsSynchronized;
            }
        }

        object ICollection.SyncRoot {
            get {
                return EnsureCollection ().SyncRoot;
            }
        }

        ICollection<string> IDictionary<string, JsonData>.Keys
        {
            get {
                EnsureDictionary ();
                IList<string> keys = objectList.Select(entry => entry.Key).ToList();

                return (ICollection<string>) keys;
            }
        }

        ICollection<JsonData> IDictionary<string, JsonData>.Values
        {
            get {
                EnsureDictionary ();
                IList<JsonData> values = objectList.Select(entry => entry.Value).ToList();

                return (ICollection<JsonData>) values;
            }
        }

        bool IJsonWrapper.IsArray {
            get { return IsArray; }
        }

        bool IJsonWrapper.IsBoolean {
            get { return IsBoolean; }
        }

        bool IJsonWrapper.IsDouble {
            get { return IsDouble; }
        }

        bool IJsonWrapper.IsInt {
            get { return IsInt; }
        }

        bool IJsonWrapper.IsLong {
            get { return IsLong; }
        }

        bool IJsonWrapper.IsObject {
            get { return IsObject; }
        }

        bool IJsonWrapper.IsString {
            get { return IsString; }
        }

        bool IList.IsFixedSize {
            get {
                return EnsureList ().IsFixedSize;
            }
        }

        bool IList.IsReadOnly {
            get {
                return EnsureList ().IsReadOnly;
            }
        }

        JsonData IDictionary<string, JsonData>.this[string key]
        {
           get {
                return EnsureDictionary ()[key];
            }

            set
            {
                if (! (key is String))
                    throw new ArgumentException (
                        "The key has to be a string");

                JsonData data = ToJsonData (value);

                this[key] = data;
            }
        }

        object IOrderedDictionary.this[int idx] {
            get {
                EnsureDictionary ();
                return objectList[idx].Value;
            }

            set {
                EnsureDictionary ();
                JsonData data = ToJsonData (value);

                KeyValuePair<string, JsonData> oldEntry = objectList[idx];

                InstObject[oldEntry.Key] = data;

                var entry = new KeyValuePair<string, JsonData> (oldEntry.Key, data);

                objectList[idx] = entry;
            }
        }

        object IList.this[int index] {
            get {
                return EnsureList ()[index];
            }

            set {
                EnsureList ();
                JsonData data = ToJsonData (value);

                this[index] = data;
            }
        }

        public JsonData this[string propName] {
            get {
                EnsureDictionary ();
                return InstObject[propName];
            }

            set {
                EnsureDictionary ();

                var entry = new KeyValuePair<string, JsonData> (propName, value);

                if (InstObject.ContainsKey (propName)) {
                    for (int i = 0; i < objectList.Count; i++) {
                        if (objectList[i].Key == propName) {
                            objectList[i] = entry;
                            break;
                        }
                    }
                } else
                    objectList.Add (entry);

                InstObject[propName] = value;

                json = null;
            }
        }

        public JsonData this[int index] {
            get {
                EnsureCollection ();

                if (type == JsonType.Array)
                    return instArray[index];

                return objectList[index].Value;
            }

            set {
                EnsureCollection ();

                if (type == JsonType.Array)
                    instArray[index] = value;
                else {
                    KeyValuePair<string, JsonData> entry = objectList[index];
                    var newEntry = new KeyValuePair<string, JsonData> (entry.Key, value);

                    objectList[index] = newEntry;
                    InstObject[entry.Key] = value;
                }

                json = null;
            }
        }

        public JsonData ()
        {
        }

        public JsonData (bool boolean)
        {
            type = JsonType.Boolean;
            instBoolean = boolean;
        }

        public JsonData (double number)
        {
            type = JsonType.Double;
            instDouble = number;
        }

        public JsonData (int number)
        {
            type = JsonType.Int;
            instInt = number;
        }

        public JsonData(long number)
        {
            type = JsonType.Long;
            instLong = number;
        }

        public JsonData (decimal number)
        {
            type = JsonType.Decimal;
            instDecimal = number;
        }

        public JsonData(ulong number)
        {
            type = JsonType.ULong;
            instUlong = number;
        }

        public JsonData (object obj)
        {
            if (obj is Boolean) {
                type = JsonType.Boolean;
                instBoolean = (bool) obj;
                return;
            }

            if (obj is Double) {
                type = JsonType.Double;
                instDouble = (double) obj;
                return;
            }

            if (obj is Int32) {
                type = JsonType.Int;
                instInt = (int) obj;
                return;
            }

            if (obj is Int64) {
                type = JsonType.Long;
                instLong = (long) obj;
                return;
            }

            if (obj is Decimal)
            {
                type = JsonType.Decimal;
                instDecimal = (decimal)obj;
                return;
            }

            if (obj is UInt64)
            {
                type = JsonType.ULong;
                instUlong = (ulong)obj;
                return;
            }

            var s = obj as string;
            if (s != null) {
                type = JsonType.String;
                instString = s;
                return;
            }

            throw new ArgumentException (
                "Unable to wrap the given object with JsonData");
        }

        public JsonData (string str)
        {
            type = JsonType.String;
            instString = str;
        }

        public static implicit operator JsonData (Boolean data)
        {
            return new JsonData (data);
        }

        public static implicit operator JsonData (Double data)
        {
            return new JsonData (data);
        }

        public static implicit operator JsonData (Int32 data)
        {
            return new JsonData (data);
        }

        public static implicit operator JsonData (Int64 data)
        {
            return new JsonData (data);
        }

        public static implicit operator JsonData(Decimal data)
        {
            return new JsonData(data);
        }

        public static implicit operator JsonData(UInt64 data)
        {
            return new JsonData(data);
        }

        public static implicit operator JsonData(String data)
        {
            return new JsonData (data);
        }

        public static explicit operator Boolean (JsonData data)
        {
            if (data.type != JsonType.Boolean)
                throw new InvalidCastException (
                    "Instance of JsonData doesn't hold a double");

            return data.instBoolean;
        }

        public static explicit operator Double (JsonData data)
        {
            if (data.type != JsonType.Double)
                throw new InvalidCastException (
                    "Instance of JsonData doesn't hold a double");

            return data.instDouble;
        }

        public static explicit operator Int32 (JsonData data)
        {
            if (data.type != JsonType.Int)
                throw new InvalidCastException (
                    "Instance of JsonData doesn't hold an int");

            return data.instInt;
        }

        public static explicit operator Int64 (JsonData data)
        {
            if (data.type != JsonType.Long)
                throw new InvalidCastException (
                    "Instance of JsonData doesn't hold a long");

            return data.instLong;
        }

        public static explicit operator Decimal(JsonData data)
        {
            if (data.type != JsonType.Decimal && data.type != JsonType.Double)
                throw new InvalidCastException(
                    "Instance of JsonData doesn't hold a decimal");

            if (data.type == JsonType.Double)
            {
                return (decimal) data.instDouble;
            }

            return data.instDecimal;
        }

        public static explicit operator UInt64(JsonData data)
        {
            if (data.type != JsonType.ULong && 
                data.type != JsonType.Long &&
                data.type != JsonType.Int)
                throw new InvalidCastException(
                    "Instance of JsonData doesn't hold a ulong");

            if (data.type == JsonType.Int)
            {
                return (ulong)data.instInt;
            }

            if (data.type == JsonType.Long)
            {
                return (ulong)data.instLong;
            }

            return data.instUlong;
        }

        public static explicit operator String (JsonData data)
        {
            if (data.type != JsonType.String)
                throw new InvalidCastException (
                    "Instance of JsonData doesn't hold a string");

            return data.instString;
        }

        void ICollection.CopyTo (Array array, int index)
        {
            EnsureCollection ().CopyTo (array, index);
        }

        void IDictionary<string, JsonData>.Add (string key, JsonData value)
        {
            JsonData data = ToJsonData (value);

            EnsureDictionary ().Add (key, data);

            var entry = new KeyValuePair<string, JsonData>(key, data);
            objectList.Add(entry);

            json = null;
        }

        bool IDictionary<string, JsonData>.ContainsKey(string key)
        {
            return EnsureDictionary().ContainsKey(key);
        }

        bool IDictionary<string, JsonData>.Remove(string key)
        {
            bool removed = EnsureDictionary ().Remove (key);

            for (int i = 0; i < objectList.Count; i++) {
                if (objectList[i].Key == key)
                {
                    objectList.RemoveAt(i);
                    break;
                }
            }

            json = null;

            return removed;
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return EnsureCollection ().GetEnumerator ();
        }

        bool IJsonWrapper.GetBoolean ()
        {
            if (type != JsonType.Boolean)
                throw new InvalidOperationException (
                    "JsonData instance doesn't hold a boolean");

            return instBoolean;
        }

        double IJsonWrapper.GetDouble ()
        {
            if (type != JsonType.Double)
                throw new InvalidOperationException (
                    "JsonData instance doesn't hold a double");

            return instDouble;
        }

        int IJsonWrapper.GetInt ()
        {
            if (type != JsonType.Int)
                throw new InvalidOperationException (
                    "JsonData instance doesn't hold an int");

            return instInt;
        }

        long IJsonWrapper.GetLong ()
        {
            if (type != JsonType.Long)
                throw new InvalidOperationException (
                    "JsonData instance doesn't hold a long");

            return instLong;
        }

        string IJsonWrapper.GetString ()
        {
            if (type != JsonType.String)
                throw new InvalidOperationException (
                    "JsonData instance doesn't hold a string");

            return instString;
        }

        decimal IJsonWrapper.GetDecimal()
        {
            if (type != JsonType.Decimal && type != JsonType.Double)
            {
                throw new InvalidOperationException(
                    "JsonData instance doesn't hold a decimal");
            }

            if (type == JsonType.Double)
            {
                return (decimal)instDouble;
            }

            return instDecimal;
        }

        ulong IJsonWrapper.GetUlong()
        {
            if (type != JsonType.ULong && 
                type != JsonType.Long &&
                type != JsonType.Int)
            {
                throw new InvalidOperationException(
                    "JsonData instance doesn't hold a ulong");
            }

            if (type == JsonType.Int)
            {
                return (ulong)instInt;
            }

            if (type == JsonType.Long)
            {
                return (ulong)instLong;
            }

            return instUlong;
        }

        void IJsonWrapper.SetBoolean (bool val)
        {
            type = JsonType.Boolean;
            instBoolean = val;
            json = null;
        }

        void IJsonWrapper.SetDouble (double val)
        {
            type = JsonType.Double;
            instDouble = val;
            json = null;
        }

        void IJsonWrapper.SetInt (int val)
        {
            type = JsonType.Int;
            instInt = val;
            json = null;
        }

        void IJsonWrapper.SetLong (long val)
        {
            type = JsonType.Long;
            instLong = val;
            json = null;
        }

        void IJsonWrapper.SetString (string val)
        {
            type = JsonType.String;
            instString = val;
            json = null;
        }

        public void SetDecimal(decimal val)
        {
            type = JsonType.Decimal;
            instDecimal = val;
            json = null;
        }

        public void SetUlong(ulong val)
        {
            type = JsonType.ULong;
            instUlong = val;
            json = null;
        }

        string IJsonWrapper.ToJson ()
        {
            return ToJson ();
        }

        void IJsonWrapper.ToJson (JsonWriter writer)
        {
            ToJson (writer);
        }

        int IList.Add (object value)
        {
            return Add (value);
        }

        void IList.Clear ()
        {
            EnsureList ().Clear ();
            json = null;
        }

        bool IList.Contains (object value)
        {
            return EnsureList ().Contains (value);
        }

        int IList.IndexOf (object value)
        {
            return EnsureList ().IndexOf (value);
        }

        void IList.Insert (int index, object value)
        {
            EnsureList ().Insert (index, value);
            json = null;
        }

        void IList.Remove (object value)
        {
            EnsureList ().Remove (value);
            json = null;
        }

        void IList.RemoveAt (int index)
        {
            EnsureList ().RemoveAt (index);
            json = null;
        }

        IDictionaryEnumerator IOrderedDictionary.GetEnumerator ()
        {
            EnsureDictionary ();

            return new OrderedDictionaryEnumerator (
                objectList.GetEnumerator ());
        }

        void IOrderedDictionary.Insert (int idx, object key, object value)
        {
            var property = (string) key;
            JsonData data  = ToJsonData (value);

            this[property] = data;

            var entry = new KeyValuePair<string, JsonData> (property, data);

            objectList.Insert (idx, entry);
        }

        void IOrderedDictionary.RemoveAt (int idx)
        {
            EnsureDictionary ();

            InstObject.Remove (objectList[idx].Key);
            objectList.RemoveAt (idx);
        }

        private ICollection EnsureCollection ()
        {
            if (type == JsonType.Array)
                return (ICollection) instArray;

            if (type == JsonType.Object)
                return (ICollection) InstObject;

            throw new InvalidOperationException (
                "The JsonData instance has to be initialized first");
        }

        private IDictionary<string, JsonData> EnsureDictionary ()
        {
            if (type == JsonType.Object)
                return InstObject;

            if (type != JsonType.None)
                throw new InvalidOperationException (
                    "Instance of JsonData is not a dictionary");

            type = JsonType.Object;
            InstObject = new Dictionary<string, JsonData> ();
            objectList = new List<KeyValuePair<string, JsonData>> ();

            return InstObject;
        }

        private IList EnsureList ()
        {
            if (type == JsonType.Array)
                return (IList) instArray;

            if (type != JsonType.None)
                throw new InvalidOperationException (
                    "Instance of JsonData is not a list");

            type = JsonType.Array;
            instArray = new List<JsonData> ();

            return (IList) instArray;
        }

        private JsonData ToJsonData (object obj)
        {
            if (obj == null)
                return null;

            var jsonData = obj as JsonData;
            if (jsonData != null) return jsonData;

            return new JsonData (obj);
        }

        private static void WriteJson (IJsonWrapper obj, JsonWriter writer)
        {
            if (obj.IsString) {
                writer.Write (obj.GetString ());
                return;
            }

            if (obj.IsBoolean) {
                writer.Write (obj.GetBoolean ());
                return;
            }

            if (obj.IsDouble) {
                writer.Write (obj.GetDouble ());
                return;
            }

            if (obj.IsInt) {
                writer.Write (obj.GetInt ());
                return;
            }

            if (obj.IsLong) {
                writer.Write (obj.GetLong ());
                return;
            }

            if (obj.IsDecimal)
            {
                writer.Write(obj.GetDecimal());
                return;
            }

            if (obj.IsULong)
            {
                writer.Write(obj.GetUlong());
                return;
            }

            if (obj.IsArray)
            {
                writer.WriteArrayStart ();
                foreach (object elem in (IList) obj)
                    WriteJson ((JsonData) elem, writer);
                writer.WriteArrayEnd ();

                return;
            }

            if (obj.IsObject) {
                writer.WriteObjectStart ();

                foreach (var entry in ((IDictionary<string, JsonData>) obj))
                {
                    writer.WritePropertyName(entry.Key);
                    WriteJson(entry.Value, writer);
                }
                writer.WriteObjectEnd ();
            }
        }


        public int Add (object value)
        {
            JsonData data = ToJsonData (value);

            json = null;

            return EnsureList ().Add (data);
        }

        public void Clear ()
        {
            if (IsObject) {
                ((IDictionary<string, JsonData>)this).Clear();
                return;
            }

            if (IsArray) {
                ((IList) this).Clear ();
            }
        }

        public bool Equals (JsonData x)
        {
            if (x == null)
                return false;

            if (x.type != type)
                return false;

            switch (type) {
            case JsonType.None:
                return true;

            case JsonType.Object:
                return InstObject.Equals (x.InstObject);

            case JsonType.Array:
                return instArray.Equals (x.instArray);

            case JsonType.String:
                return instString.Equals (x.instString);

            case JsonType.Int:
                return instInt.Equals (x.instInt);

            case JsonType.Long:
                return instLong.Equals (x.instLong);

            case JsonType.Double:
                return instDouble.Equals (x.instDouble);

            case JsonType.Boolean:
                return instBoolean.Equals (x.instBoolean);

            case JsonType.Decimal:
                return instDecimal.Equals(x.instDecimal);

            case JsonType.ULong:
                return instUlong.Equals(x.instUlong);
            }

            return false;
        }

        public JsonType GetJsonType ()
        {
            return type;
        }

        public void SetJsonType (JsonType jsonType)
        {
            if (type == jsonType)
                return;

            switch (jsonType) {
            case JsonType.None:
                break;

            case JsonType.Object:
                InstObject = new Dictionary<string, JsonData> ();
                objectList = new List<KeyValuePair<string, JsonData>> ();
                break;

            case JsonType.Array:
                instArray = new List<JsonData> ();
                break;

            case JsonType.String:
                instString = default (String);
                break;

            case JsonType.Int:
                instInt = default (Int32);
                break;

            case JsonType.Long:
                instLong = default (Int64);
                break;

            case JsonType.Double:
                instDouble = default (Double);
                break;

            case JsonType.Boolean:
                instBoolean = default (Boolean);
                break;

            case JsonType.Decimal:
                instDecimal = default(Decimal);
                break;

            case JsonType.ULong:
                instUlong = default(UInt64);
                break;
            }

            type = jsonType;
        }

        public string ToJson ()
        {
            if (json != null)
                return json;

            var sw = new StringWriter ();
            var writer = new JsonWriter(sw) {Validate = false};

            WriteJson (this, writer);
            json = sw.ToString ();

            return json;
        }

        public void ToJson (JsonWriter writer)
        {
            bool oldValidate = writer.Validate;

            writer.Validate = false;

            WriteJson (this, writer);

            writer.Validate = oldValidate;
        }

        public override string ToString ()
        {
            switch (type) {
            case JsonType.Array:
                return "JsonData array";

            case JsonType.Boolean:
                return instBoolean.ToString ();

            case JsonType.Double:
                return instDouble.ToString (CultureInfo.InvariantCulture);

            case JsonType.Int:
                return instInt.ToString (CultureInfo.InvariantCulture);

            case JsonType.Long:
                return instLong.ToString (CultureInfo.InvariantCulture);

            case JsonType.Decimal:
                return instDecimal.ToString(CultureInfo.InvariantCulture);

            case JsonType.ULong:
                return instUlong.ToString(CultureInfo.InvariantCulture);

            case JsonType.Object:
                return "JsonData object";

            case JsonType.String:
                return instString;
            }

            return "Uninitialized JsonData";
        }


        public bool TryGetValue(string key, out JsonData value)
        {
            return EnsureDictionary().TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<string, JsonData> item)
        {
            EnsureDictionary().Add(item);
        }

        public bool Contains(KeyValuePair<string, JsonData> item)
        {
            return EnsureDictionary().Contains(item);
        }

        public void CopyTo(KeyValuePair<string, JsonData>[] array, int arrayIndex)
        {
            EnsureDictionary().CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, JsonData>>.IsReadOnly
        {
            get { return EnsureDictionary().IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, JsonData> item)
        {
            return EnsureDictionary().Remove(item.Key);
        }

        IEnumerator<KeyValuePair<string, JsonData>> IEnumerable<KeyValuePair<string, JsonData>>.GetEnumerator()
        {
            return EnsureDictionary().GetEnumerator();
        }
    }

    internal class OrderedDictionaryEnumerator : IDictionaryEnumerator
    {
        readonly IEnumerator<KeyValuePair<string, JsonData>> listEnumerator;

        public object Current {
            get { return Entry; }
        }

        public DictionaryEntry Entry {
            get {
                KeyValuePair<string, JsonData> curr = listEnumerator.Current;
                return new DictionaryEntry (curr.Key, curr.Value);
            }
        }

        public object Key {
            get { return listEnumerator.Current.Key; }
        }

        public object Value {
            get { return listEnumerator.Current.Value; }
        }


        public OrderedDictionaryEnumerator (
            IEnumerator<KeyValuePair<string, JsonData>> enumerator)
        {
            listEnumerator = enumerator;
        }


        public bool MoveNext ()
        {
            return listEnumerator.MoveNext ();
        }

        public void Reset ()
        {
            listEnumerator.Reset ();
        }
    }
}
