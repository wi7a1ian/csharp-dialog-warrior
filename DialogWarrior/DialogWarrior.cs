using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace Utils
{

    /**
     * http://msdn.microsoft.com/en-us/library/ms182161(v=vs.110).aspx
     */
    internal static class NativeMethods
    {
        // Delegate for EnumWindows callback.
        internal delegate bool EnumWinCallBack(IntPtr handle, IntPtr lParam);
        // Delegate for EnumChildWindows callback.
        internal delegate bool EnumChildProc(IntPtr handle, IntPtr lParam);
        // Delegate for EnumDestktopWindows callback.
        internal delegate bool EnumWindowsProc(IntPtr handle, IntPtr lParam);

        // Windows messages.
        internal enum MessageNumber : uint
        {
            WM_USER = 0x0400,

            WM_SETFOCUS = 0x0007,
            WM_ENABLE = 0x000A,
            WM_NEXTDLGCTL = 0x0028,
            WM_ACTIVATE = 0x0006,

            // Key press 
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,

            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,

            // Window text 
            WM_SETTEXT = 0x000C,
            WM_GETTEXT = 0x000D,
            WM_GETTEXTLENGTH = 0x000E,

            // Set button check
            BM_SETCHECK = 0x00F1,

            // Select from text in a combo box.
            CB_SELECTSTRING = 0x014D,

            // Close 
            WM_CLOSE = 0x0010,
            // Or...
            WM_SYSCOMMAND = 0x0112,
            SC_CLOSE = 0xF060

        }

        [DllImport("User32.dll", EntryPoint = "FindWindow", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool PostMessage(IntPtr handle, UInt32 message, IntPtr wparam, IntPtr lparam);

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SendMessage(IntPtr handle, int message, IntPtr wparam, IntPtr lparam);

        [DllImport("User32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumWindows(EnumWinCallBack callBackFunc, IntPtr lParam);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumChildWindows(IntPtr parentHandle, EnumChildProc callback, IntPtr lParam);

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal extern static bool EnumThreadWindows(int threadId, EnumWindowsProc callback, IntPtr lParam);

        [DllImport("User32.Dll")]
        internal static extern int GetWindowThreadProcessId(IntPtr handle, out uint processId);
        [DllImport("User32.Dll")]// When you don't want the ProcessId, use this overload and pass IntPtr.Zero for the second parameter
        internal static extern int GetWindowThreadProcessId(IntPtr handle, IntPtr Zero);

        [DllImport("User32.Dll")]
        internal static extern bool IsWindow(IntPtr handle);

        [DllImport("User32.dll")]
        internal static extern bool IsWindowVisible(IntPtr handle);

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        internal static extern int GetWindowText(IntPtr handle, StringBuilder buffer, int bufferSize);
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        internal static extern int GetWindowText(IntPtr handle, IntPtr buffer, int bufferSize);

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        internal static extern int GetClassName(IntPtr handle, IntPtr buffer, int bufferSize);

        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);
    }


    /**
     * This brave warrior can kill any type of dragon (DialogBox/MessageWindow).
     * He can simply use his sword to kill the dragon (close the window),
     * use one of the spells he learned in the academy (press "Yes"/"No"/"Cancel")
     * or you can teach him some super spell (custom button name).
     * 
     * Give him quests and he will embark on never ending journey! For the glory of brave ones!
     * 
     * 12/01/2013   Zajac Witalian	v1.0.0  Initial Release
     * 12/09/2013   Zajac Witalian	v1.1.0  Using IDisposable interface 
     */
    public class DialogWarrior : IDisposable
    {
        // Track whether Dispose has been called. 
        private bool _disposed = false;

        private Thread _journey;
        private volatile bool _isAliveAndKicking = false;
        public List<QuestItem> questLog { get; private set; }
        private QuestItem _currentQuest;

        public enum QuestType
        {
            KillTheDragon,// Close the window
            KillTheDragonByYesSpell,// Press "Yes"
            KillTheDragonByNoSpell,// Press "No"
            KillTheDragonByCancelSpell,// Press "Cancel"
            KillTheDragonUsingOtherSpell // Press some custom button
        }

        public class QuestItem
        {
            public string dragonToSearchFor;
            public string specialSpellToUse = string.Empty;
            public QuestType typeOfQuest;
        }

        public DialogWarrior()
        {
            _journey = new Thread(LifeLongJourney);
            questLog = new List<QuestItem>();
        }

        public void AddQuest(string dragonsName, QuestType type)
        {
            if(string.IsNullOrEmpty(dragonsName))
                throw new Exception("Thou shall name the dragon!");

            if (type != QuestType.KillTheDragonUsingOtherSpell)
                questLog.Add(new QuestItem { dragonToSearchFor = dragonsName, typeOfQuest = type });
            else
                throw new Exception("Thou shall choose different quest!");
        }

        public void AddQuest(string dragonsName, string spell)
        {
            if (string.IsNullOrEmpty(dragonsName))
                throw new Exception("Thou shall name the dragon!");

            if (string.IsNullOrEmpty(spell))
                throw new Exception("Thou shall name the spell!");

            questLog.Add(new QuestItem { dragonToSearchFor = dragonsName, specialSpellToUse = spell, typeOfQuest = QuestType.KillTheDragonUsingOtherSpell });
        }

        public void EmbarkOnAnAdventure()
        {
            _isAliveAndKicking = true;

            if (!_journey.IsAlive)
                _journey.Start();
        }

        public void Retire()
        {
            _isAliveAndKicking = false;

            if (_journey.IsAlive)
                _journey.Join(15000); // 15sec timeout
            //_journey.Abort();
        }

        private void LifeLongJourney()
        {
            while (_isAliveAndKicking)
            {
                Thread.Sleep(100); // The brave warrior should rest!
                NativeMethods.EnumWindows(EnumDragonLairs, IntPtr.Zero);
            }
        }

        /*public static void CloseWindowWithCaption(string caption)
        {
            IntPtr handle = NativeMethods.FindWindowByCaption(IntPtr.Zero, caption);
            if (handle != IntPtr.Zero)
            {
                NativeMethods.PostMessage(handle, (uint)NativeMethods.MessageNumber.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                //NativeMethods.PostMessage(handle, (uint)NativeMethods.MessageNumber.WM_SYSCOMMAND, (IntPtr)NativeMethods.MessageNumber.SC_CLOSE, IntPtr.Zero);
            }
        }*/

        private bool EnumDragonLairs(IntPtr handle, IntPtr pointer)
        {

            if (NativeMethods.IsWindowVisible(handle))
            {
                StringBuilder dragonsNameB = new StringBuilder(1024);
                NativeMethods.GetWindowText(handle, dragonsNameB, dragonsNameB.Capacity);
                string dragonsName = dragonsNameB.ToString().ToLower();

                foreach (QuestItem quest in questLog)
                {
                    if (string.Equals(dragonsName, quest.dragonToSearchFor, StringComparison.InvariantCultureIgnoreCase)
                        || (quest.dragonToSearchFor.Length > 0 && dragonsName.Contains(quest.dragonToSearchFor.ToLower())))
                    {
                        _currentQuest = quest;
                        if (quest.typeOfQuest == QuestType.KillTheDragon)
                        {
                            NativeMethods.PostMessage(handle, (uint)NativeMethods.MessageNumber.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                            //NativeMethods.PostMessage(handle, (uint)NativeMethods.MessageNumber.WM_SYSCOMMAND, (IntPtr)NativeMethods.MessageNumber.SC_CLOSE, IntPtr.Zero);
                        }
                        else
                        {
                            // Loop though the child windows, and execute the EnumChildWindowsCallback method
                            NativeMethods.EnumChildWindows(handle, EnumSpellsInSpellBook, IntPtr.Zero);
                        }
                    }
                }
            }

            return true;
        }

        private bool EnumSpellsInSpellBook(IntPtr handle, IntPtr pointer)
        {
            StringBuilder spellB = new StringBuilder(1024);

            // Get the control's text.
            NativeMethods.GetWindowText(handle, spellB, spellB.Capacity);
            string spell = spellB.ToString();

            string buttonText = string.Empty;
            if (_currentQuest.typeOfQuest == QuestType.KillTheDragonByNoSpell)
                buttonText = @"&No";
            else if (_currentQuest.typeOfQuest == QuestType.KillTheDragonByYesSpell)
                buttonText = @"&Yes";
            else if (_currentQuest.typeOfQuest == QuestType.KillTheDragonByCancelSpell)
                buttonText = @"Cancel";
            else if (_currentQuest.typeOfQuest == QuestType.KillTheDragonUsingOtherSpell)
                buttonText = _currentQuest.specialSpellToUse;
            else return false; // Dragon says: How dare you coming here without proper knowledge! Poof!

            // If the text on the control == &No send a left mouse click to the handle.
            if (spell == buttonText)
            {
                NativeMethods.PostMessage(handle, (uint)NativeMethods.MessageNumber.WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
                NativeMethods.PostMessage(handle, (uint)NativeMethods.MessageNumber.WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
                return false; // Stop enumeration
            }
            else
            {
                return true; // Continue search
            }

        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to take this object off the finalization queue 
            // and prevent finalization code for this object from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly or indirectly by a user's code.
        // Managed and unmanaged resources can be disposed.
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed and unmanaged resources. 
                if (disposing)
                {
                    // None yet, or maybe window handles?
                }

                // Call the appropriate methods to clean up unmanaged resources here. 
                // If disposing is false, only the following code is executed.
                Retire();

                // Note disposing has been done.
                _disposed = true;
            }
        }

        ~DialogWarrior()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }

        #endregion Dispose
    }
}
