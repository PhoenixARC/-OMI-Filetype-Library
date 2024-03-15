/* Copyright (c) 2022-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.Collections.Generic;
using System.Linq;

namespace OMI.Formats.Languages
{
    public class LOCFile
    {
        public class InvalidLanguageException : Exception
        {
            public string Language { get; }
            public InvalidLanguageException(string message, string language) : base(message)
            {
                Language = language;
            }
        }

        public static readonly string[] ValidLanguages = new string[]
        {
            "cs-CS",
            "cs-CZ",

            "da-CH",
            "da-DA",
            "da-DK",

            "de-AT",
            "de-DE",

            "el-EL",
            "el-GR",

            "en-AU",
            "en-CA",
            "en-EN",
            "en-GB",
            "en-GR",
            "en-IE",
            "en-NZ",
            "en-US",

            "es-ES",
            "es-MX",

            "fi-BE",
            "fi-CH",
            "fi-FI",

            "fr-FR",
            "fr-CA",

            "it-IT",

            "ja-JP",

            "ko-KR",

            "la-LAS",

            "no-NO",

            "nb-NO",

            "nl-NL",
            "nl-BE",

            "pl-PL",

            "pt-BR",
            "pt-PT",

            "ru-RU",

            "sk-SK",

            "sv-SE",

            "tr-TR",

            "zh-CN",
            "zh-HK",
            "zh-SG",
            "zh-TW",
            "zh-CHT",
            "zh-HanS",
            "zh-HanT",
        };

        private Dictionary<string, Dictionary<string, string>> _lockeys = new Dictionary<string, Dictionary<string, string>>();
        private List<string> _languages = new List<string>(ValidLanguages.Length);
        
        internal bool hasUids = false;

        public Dictionary<string, Dictionary<string, string>> LocKeys => _lockeys;
        public List<string> Languages => _languages;

        private Dictionary<string, string> GetTranslation(string locKey)
        {
            if (!LocKeys.ContainsKey(locKey))
                LocKeys.Add(locKey, new Dictionary<string, string>());
            return LocKeys[locKey];
        }

        public Dictionary<string, string> GetLocEntries(string locKey)
        {
            if (!LocKeys.ContainsKey(locKey))
                throw new KeyNotFoundException("Loc key not found");
            return LocKeys[locKey];
        }

        public bool HasLocEntry(string locKey)
            => LocKeys.ContainsKey(locKey);

        public string GetLocEntry(string locKey, string language)
        {
            if (!LocKeys.ContainsKey(locKey))
                throw new KeyNotFoundException(nameof(locKey));
            if (!Languages.Contains(language)) throw new KeyNotFoundException("Language Entry not found");
            return GetTranslation(locKey)[language]?? string.Empty;
        }

        public void SetLocEntry(string locKey, string value)
        {
            foreach (var language in Languages)
            {
                GetTranslation(locKey)[language] = value;
            }
        }

        public void SetLocEntry(string locKey, string language, string value)
        {
            if (!Languages.Contains(language))
                throw new KeyNotFoundException(nameof(language));
            GetTranslation(locKey)[language] = value;
        }

        public bool AddLocKey(string locKey, string value)
        {
            if (LocKeys.ContainsKey(locKey))
                return false;
            Languages.ForEach( language => SetLocEntry(locKey, language, value) );
            return true;
        }

        public bool RemoveLocKey(string locKey)
        {
            if (!LocKeys.ContainsKey(locKey))
                return false;
            return LocKeys.Remove(locKey);
        }

        public void AddLanguage(string language)
        {
            if (!ValidLanguages.Contains(language))
                throw new InvalidLanguageException("Invalid language", language);
            if (Languages.Contains(language))
                throw new InvalidLanguageException("Language already exists", language);
            Languages.Add(language);
            foreach(var key in LocKeys.Keys)
                SetLocEntry(key, language, "");
        }

        public void RemoveLanguage(string language)
        {
            if (!ValidLanguages.Contains(language))
                throw new InvalidLanguageException("Invalid language", language);
            if (!Languages.Contains(language))
                throw new InvalidLanguageException("Language doesn't exist", language);
            if (Languages.Remove(language))
                foreach (var translation in LocKeys.Values)
                    translation.Remove(language);
        }
    }
}
