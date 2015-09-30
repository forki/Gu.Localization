﻿namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Gu.Localization.Properties;

    public class Translator : ITranslator
    {
        private static CultureInfo _currentCulture;

        static Translator()
        {
            LanguageManager.LanguagesChanged += (_, __) => OnLanguagesChanged();
        }

        public static event EventHandler<CultureInfo> CurrentLanguageChanged;

        public static event EventHandler<EventArgs> LanguagesChanged;

        /// <summary>
        /// The culture to translate to
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get
            {
                if (_currentCulture != null)
                {
                    return _currentCulture;
                }
                CurrentCulture = AllCultures.FirstOrDefault();
                return _currentCulture;
            }
            set
            {
                if (Equals(_currentCulture, value))
                {
                    return;
                }
                _currentCulture = value;
                OnLanguageChanged(value);
            }
        }

        public static IReadOnlyList<CultureInfo> AllCultures => LanguageManager.AllCultures;

        CultureInfo ITranslator.CurrentCulture
        {
            get
            {
                return CurrentCulture;
            }
            set
            {
                CurrentCulture = value;
            }
        }

        IReadOnlyList<CultureInfo> ITranslator.AllCultures => AllCultures;

        public static string Translate(Expression<Func<string>> key)
        {
            if (ExpressionHelper.IsResourceKey(key))
            {
                var type = ExpressionHelper.GetRootType(key);
                var resourceKey = ExpressionHelper.GetResourceKey(key);
                return Translate(type, resourceKey);
            }
            return key.Compile().Invoke();
        }

        public static string Translate(Type typeInAsembly, string key)
        {
            if (typeInAsembly == null)
            {
                throw new ArgumentNullException(nameof(typeInAsembly));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(key);
            }

            var translate = Translate(typeInAsembly.Assembly, key);
            if (translate != null)
            {
                return translate;
            }
            return string.Format(Properties.Resources.MissingTranslationFormat, key);
        }

        public static string Translate(Assembly assembly, string key)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(key);
            }

            var manager = LanguageManager.GetOrCreate(assembly);

            if (manager == null)
            {
                return string.Format(Resources.NullManagerFormat, key);
            }
            if (string.IsNullOrEmpty(key))
            {
                return "null";
            }
            return manager.Translate(CurrentCulture, key);
        }

        string ITranslator.Translate(Expression<Func<string>> key) => Translate(key);

        string ITranslator.Translate(Type typeInAsembly, string key) => Translate(typeInAsembly, key);

        private static void OnLanguageChanged(CultureInfo e)
        {
            CurrentLanguageChanged?.Invoke(null, e);
        }

        private static void OnLanguagesChanged()
        {
            LanguagesChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}
