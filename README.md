# Info
    This brave warrior can kill any type of dragon (DialogBox/MessageWindow).
    He can simply use his sword to kill the dragon (close the window),
    use one of the spells he learned in the academy (press "Yes"/"No"/"Cancel")
    or you can teach him some super spell (target custom button name).

    Give him quests and he will embark on never ending journey! For the glory of brave ones!

# Usage
```csharp
using(var finn = new DialogWarrior())
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
```
