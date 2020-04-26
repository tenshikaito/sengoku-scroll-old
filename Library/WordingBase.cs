﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class WordingBase
    {
        protected Dictionary<string, string> data;

        public string locale { get; }

        public virtual string this[string key] => data.TryGetValue(key, out var value) ? value : key;

        public WordingBase(string locale, Dictionary<string, string> data)
        {
            this.locale = locale;
            this.data = data;
        }

        public class Part
        {
            protected WordingBase wording;
            protected string prefix;

            public string this[string key] => wording.data.TryGetValue($"{prefix}.{key}", out var value) ? value : key;

            public Part(WordingBase w, string prefix)
            {
                wording = w;

                this.prefix = prefix;
            }
        }

        public sealed class Association
        {
            private Dictionary<string, string> dictionary = new Dictionary<string, string>();
            private string format;

            public Association(string format) => this.format = format;

            public string this[string name]
                => dictionary.TryGetValue(name, out var value) ? value : dictionary[name] = string.Format(format, name);
        }

        //public class Word
        //{
        //    public string index { get; }
        //    public string content { get; }

        //    public Word(string content) : this(content, content)
        //    {

        //    }

        //    public Word(string index, string content)
        //    {
        //        this.index = index;
        //        this.content = content;
        //    }

        //    public override string ToString() => content;

        //    public static implicit operator Word(string content) => new Word(content);

        //    public static implicit operator string(Word w) => w.content;
        //}
    }
}
