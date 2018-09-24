using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kroll.CDG.Fixer.Utils;
using System.Windows.Forms;

namespace UTDialogWarrior
{
    class Program
    {
        static void Main(string[] args)
        {
            DialogWarrior warrior = new DialogWarrior();
            warrior.AddQuest("Quest 1", DialogWarrior.QuestType.KillTheDragon);
            warrior.AddQuest("Quest 2", DialogWarrior.QuestType.KillTheDragonByNoSpell);
            warrior.AddQuest("Quest", DialogWarrior.QuestType.KillTheDragonByCancelSpell);
            warrior.EmbarkOnAJourney();

            System.Windows.Forms.MessageBox.Show("Wild dragon appears!", "Quest 1", MessageBoxButtons.OK);
            System.Windows.Forms.MessageBox.Show("Wild dragon appears!", "Quest 2", MessageBoxButtons.YesNo);
            System.Windows.Forms.MessageBox.Show("Wild dragon appears!", "Quest 3", MessageBoxButtons.YesNoCancel);

            warrior.Retire();
        }
    }
}
