using System.Collections.Generic;
using KontrolSystem.SpaceWarpMod.Utils;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod.UI {
public class FontManager : Singleton<FontManager> {
        public static readonly string[] DefaultConsoleFonts = {
            "User pick Goes Here", // overwrite this first one with the user selection - the rest are a fallback just in case
            "Consolas Bold", // typical Windows good programming font
            "Consolas",
            "Monaco Bold", // typical Mac good programming font
            "Monaco",
            "Liberation Mono Bold", // typical Linux good programming font
            "Liberation Mono",
            "Courier New Bold", // The Courier ones are ugly fallbacks just in case.
            "Courier Bold",
            "Courier New",
            "Courier",
            "Arial" // very bad, proportional, but guaranteed to exist in Unity no matter what.
        };

        /// <summary>
        /// All the fonts that were loaded by kOS will be in this dictionary,
        /// with font names as the keys.  It is possible for the same font
        /// to be in the dictionary more than once if it has multiple alias names.
        /// </summary>
        private readonly Dictionary<string, Font> fonts = new Dictionary<string, Font>();

        private readonly SortedSet<string> fontNames = new SortedSet<string>();

        void Awake() {
            UpdateSystemFontLists();

            LoggerAdapter.Instance.Debug("Awake FontManager");
        }

        public IEnumerable<string> FontNames => fontNames;

        /// <summary>
        /// Returns a Unity Font object corresponding to the system font
        /// name you give it.  This will lazy-load the font, so the first time
        /// you use a font name it may take a bit of time, and then on each
        /// subsequent call using the same name it should be fast.  Can return
        /// null if the font name you give it is no good.
        /// </summary>
        /// <returns>The font</returns>
        /// <param name="name">Name of the font as it appaers in GetSystemFontNames</param>
        /// <param name="size">point size for the desired font.</param>
        /// <param name="checkMono">If true, then perform a check for monospace and issue a warning and return null
        /// if it's not monospaced.</param>
        /// <param name="doErrorMessage">If true, then if the checkMono check (see above) fails, a message will
        /// appear on screen complaining about this as it returns a null, else it will return null silently.</param>
        /// <param name="doDetectedCheck">If true, then protect against trying to use un-detected font names.
        /// By default, if the font name you use was not discovered when walking the list of all OS font names, this
        /// method will force a null return.  But if this is being called before that walk is finished, you have to
        /// bypass that check to make it work at all.</param> 
        public Font GetSystemFontByNameAndSize(string name, int size, bool checkMono, bool doErrorMessage = true,
            bool doDetectedCheck = true) {
            // Now that a font is asked for, now we'll lazy-load it.

            // Make a string key from the name plus the size:
            string key = MakeKey(name, size);
            if ((!fonts.ContainsKey(key)) || fonts[key] == null) {
                // Font.CreateDynamicFontFromOSFont will never return null just because you
                // gave it a bogus font name that doesn't exist.  Instead Unity chooses
                // to return the default Arial font instead when it can't find the font name.
                // Because our logic requires that we try calling this method again and again
                // walking through a list of fonts until we find one that works, we need this
                // to return null when there's no such font.
                // (Else when the first font in the list of fonts to try fails, we get a
                // "success" at using Arial, instead of trying the next font.)
                if (doDetectedCheck && !fontNames.Contains(name))
                    return null;

                fonts[key] = Font.CreateDynamicFontFromOSFont(name, size);
            }

            Font potentialReturn = fonts[key];
            if (checkMono && !(IsFontMonospaced(potentialReturn))) {
                if (doErrorMessage) {
                    string msg = string.Format("{0} is proportional width.\nA monospaced font is required.", name);
                    LoggerAdapter.Instance.Error(msg);
                }

                // Must destroy the font right now, else Unity keeps the font data temporarily alive for
                // too long a window of time, and it ends up failing when there's too many of
                // them loaded at once.  Relying on the cleanup that automatically happens when you
                // orphan the Font takes too long to avoid this bug.  As long as it *temporarily* had
                // too much font data in memory, it breaks some of the other fonts that were loaded but
                // hadn't be excercised yet.  (i.e nothing drawn in Arial with default GUI.skin can be
                // seen anymore).
                // This is a bug that during our user community testing didn't seem to happen to everybody,
                // but it happened to some users depending on kinds of fonts they had on their OS and kinds
                // of graphics cards (it seems to be related to the texture size of the texture being sent to
                // the graphics card).  At any rate, explicitly doing this instead of waiting for the
                // cleanup that is epxected to "magically" happen later on its own seems to cure the problem:
                DestroyImmediate(potentialReturn);
                fonts.Remove(key);

                return null;
            }

            return potentialReturn;
        }

        /// <summary>
        /// Just like GetSystemFontByNameAndSize, except that you give it a list of
        /// multiple names and it will try each in turn until it finds a name
        /// that returns an existing font.  Only if all the names fail to find
        /// a hit will it return null.
        /// </summary>
        /// <returns>The system font by name.</returns>
        /// <param name="names">Names.</param>
        /// <param name="size">Size.</param>
        /// <param name="checkMono">If true, then perform a check for monospace and issue a warning and return null
        /// if it's not monospaced.</param>
        public Font GetSystemFontByNameAndSize(string[] names, int size, bool checkMono) {
            foreach (string name in names) {
                Font hit = GetSystemFontByNameAndSize(name, size, checkMono);
                if (hit != null)
                    return hit;
            }

            return null;
        }

        private string MakeKey(string name, int size) => $"{name}/{size}";

        /// <summary>
        /// Query the OS to build a list of all font names currently installed.
        /// If any new fonts have been installed into the OS since the last time
        /// this was called, or any old fonts have been uninstalled, calling this
        /// should rebuild the list to reflect those changes.  This returns void,
        /// so to actually see the list, you have to call GetSystemFontNames after calling this.
        /// <para></para>
        /// <para>------------</para>
        /// <para>Be aware that this is an expensive operation that compares two lists and is O(n^2).
        /// (n = number of fonts installed on the OS so it won't be *that* big).  But still,
        /// It shouldn't be performed repetatively.</para>
        /// </summary>
        private void UpdateSystemFontLists() {
            HashSet<string> namesThatNoLongerExist = new HashSet<string>(fontNames);
            foreach (string fontName in Font.GetOSInstalledFontNames()) {
                if (!fontNames.Contains(fontName)) {
                    // Only add those fonts which pass the monospace test:
                    if (GetSystemFontByNameAndSize(fontName, 13, true, false, false) != null)
                        fontNames.Add(fontName);
                }

                namesThatNoLongerExist.Remove(fontName);
            }

            // Any font name that used to be in the list but wasn't seen this time around
            // must be a font that has been uninstalled from the OS while KSP was running:
            foreach (string goneName in namesThatNoLongerExist) {
                fontNames.Remove(goneName);
            }
        }

        /// <summary>A tool we can use to check if a font is monospaced by
        /// comparing the width of certain key letters.</summary>
        private static bool IsFontMonospaced(Font f) {
            CharacterInfo chInfo;
            int prevWidth;

            // Unity Lazy-loads the character info for the font.  Until you try
            // to actually render a character, it's CharacterInfo isn't populated
            // yet (all fields for the character's dimensions return a bogus zero value).
            // This next call forces Unity to load the information for the given characters
            // even though they haven't been rendered yet:
            f.RequestCharactersInTexture("XiW _i:");

            f.GetCharacterInfo('X', out chInfo);
            prevWidth = chInfo.advance;

            f.GetCharacterInfo('i', out chInfo);
            if (prevWidth != chInfo.advance)
                return false;
            prevWidth = chInfo.advance;

            f.GetCharacterInfo('W', out chInfo);
            if (prevWidth != chInfo.advance)
                return false;
            prevWidth = chInfo.advance;

            f.GetCharacterInfo(' ', out chInfo);
            if (prevWidth != chInfo.advance)
                return false;
            prevWidth = chInfo.advance;

            f.GetCharacterInfo('_', out chInfo);
            if (prevWidth != chInfo.advance)
                return false;
            prevWidth = chInfo.advance;

            f.GetCharacterInfo(':', out chInfo);
            if (prevWidth != chInfo.advance)
                return false;
            prevWidth = chInfo.advance;

            // That's probably a good enough test.  If all the above characters
            // have the same width, there's really good chance this is monospaced.

            return true;
        }
    }
}
