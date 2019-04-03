using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using System.Windows.Forms;

namespace Utils.DialogWarriorTests
{
    class Program
    {
        static void Main(string[] args)
        {
            using(DialogWarrior finn = new DialogWarrior())
            { 
                finn.AddQuest("Quest 1", DialogWarrior.QuestType.KillTheDragon);
                finn.AddQuest("Quest 2", DialogWarrior.QuestType.KillTheDragonByNoSpell);
                finn.AddQuest("Quest", DialogWarrior.QuestType.KillTheDragonByCancelSpell);
                finn.EmbarkOnAnAdventure();

                System.Windows.Forms.MessageBox.Show("Wild dragon appears!", "Quest 1", MessageBoxButtons.OK);
                System.Windows.Forms.MessageBox.Show("Wild dragon appears!", "Quest 2", MessageBoxButtons.YesNo);
                System.Windows.Forms.MessageBox.Show("Wild dragon appears!", "Quest 3", MessageBoxButtons.YesNoCancel);

                finn.Retire();
            }
        }
    }
}
