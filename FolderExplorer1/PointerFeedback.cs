using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderExplorer1
{
    class PointerFeedback
    {
        public static void PlaySound()
        {
            // Sound feedback for copy
            System.Media.SoundPlayer simpleSound = new System.Media.SoundPlayer("..\\..\\Music\\app.wav");
            simpleSound.Play();
        }

        public static void PlayClickSound()
        {
            // Sound feedback for copy
            System.Media.SoundPlayer simpleSound = new System.Media.SoundPlayer("..\\..\\Music\\click.wav");
            simpleSound.Play();
        }
    }
}
