﻿using System;
using System.Collections.Generic;
using System.Linq;
using Arc.YTSubConverter.Animations;
using Arc.YTSubConverter.Util;

namespace Arc.YTSubConverter.Formats.Ass.KaraokeTypes
{
    internal class GlitchKaraokeType : SimpleKaraokeType
    {
        private static readonly CharacterRange[][] LanguageCharacterRanges =
        {
            new[] { new CharacterRange('A', 'Z'), new CharacterRange('a', 'z') },
            new[] { CharacterRange.IdeographRange, CharacterRange.IdeographExtensionRange, CharacterRange.IdeographCompatibilityRange },
            new[] { CharacterRange.HiraganaRange },
            new[] { CharacterRange.KatakanaRange },
            new[] { CharacterRange.HangulRange }
        };

        private static readonly CharacterRange[] RandomCharacterRanges =
            {
                new CharacterRange('\x2300', '\x231A'),
                new CharacterRange('\x231C', '\x23E1')
            };

        public override IEnumerable<AssLine> Apply(AssKaraokeStepContext context)
        {
            AssSection singingSection = context.SingingSections.LastOrDefault(s => s.RubyPart == RubyPart.None || s.RubyPart == RubyPart.Text);
            if (singingSection == null || singingSection.Text.Length == 0)
                return new List<AssLine> { context.StepLine };

            base.Apply(context);
            DateTime glitchEndTime = TimeUtil.Min(context.StepLine.Start.AddMilliseconds(70), context.StepLine.End);
            CharacterRange[] charRanges = GetGlitchKaraokeCharacterRanges(singingSection.Text[0]);
            singingSection.Animations.Add(new GlitchingCharAnimation(context.StepLine.Start, glitchEndTime, charRanges));
            return new[] { context.StepLine };
        }

        private static CharacterRange[] GetGlitchKaraokeCharacterRanges(char c)
        {
            foreach (CharacterRange[] ranges in LanguageCharacterRanges)
            {
                if (ranges.Any(r => r.Contains(c)))
                    return ranges;
            }

            return RandomCharacterRanges;
        }
    }
}