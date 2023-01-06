using System;

namespace HerPublicWebsite.Helpers
{
    public class RandomHelper
    {
        public static string Generate8CharacterReference()
        {
            string characters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            string reference = "";

            for (int i = 0; i < 8; i++)
            {
                reference += characters[new Random().Next(0, characters.Length)];
            }
            
            return reference;
        }
    }
}