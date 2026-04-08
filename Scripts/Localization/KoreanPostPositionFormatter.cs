using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.Core.Parsing;

namespace Elysia.Localizations
{
    /// <summary>
    /// 한국어조사가 앞 글자의 받침 여부에 따라 달라지는 경우에 대응하기 위한 Formatter.<br/>
    /// ex) 주격조사 [이|가]는 받침이 있으면 [이], 없으면 [가].<br/>
    /// ex) 목적격조사 [을|를]은 받침이 있으면 [을], 없으면 [를].<br/>
    /// <br/>
    /// 부사격조사 [(으)로]는 예외이므로 <see cref="KoreanPostPositionRoFormatter"/>를 사용한다.<br/>
    /// <br/>
    /// usage:<br/>
    /// {[variable]:kor-pp:[followed-by-consonant]|[followed-by-vowel]}<br/>
    /// <br/>
    /// result:<br/>
    /// i: {학생:kor-pp:은|는}<br/>
    /// o: 학생은<br/>
    /// <br/>
    /// i: {내:kor-pp:이|가}<br/>
    /// o: 내가<br/>
    /// <br/>
    /// i: {not-korean:kor-pp:이|가}<br/>
    /// o: not-korean이(가)<br/>
    /// <br/>
    /// i: {not-korean:kor-pp:이여|여}<br/>
    /// o: not-korean(이)여<br/>
    /// </summary>
    [UsedImplicitly]
    public class KoreanPostPositionFormatter : FormatterBase
    {
        public override string[] DefaultNames => new string[] { "kor-pp" };

        private static readonly string FORMAT_OMIT_SECOND = "{0}({1})";
        private static readonly string FORMAT_OMIT_FIRST = "({0}){1}";

        public override bool TryEvaluateFormat(IFormattingInfo formattingInfo)
        {
            Format format = formattingInfo.Format;
            if (format == null || format.baseString[format.startIndex] == ':')
            {
                return false;
            }

            string str = formattingInfo.CurrentValue.ToString();
            formattingInfo.Write(str);

            IList<Format> candidatesMarkers = format.Split('|');
            if (candidatesMarkers.Count != 2)
            {
                return false;
            }

            string pp0 = candidatesMarkers[0].RawText;
            string pp1 = candidatesMarkers[1].RawText;

            if (str.Length == 0)
            {
                formattingInfo.Write(pp0.EndsWith(pp1) ? string.Format(FORMAT_OMIT_FIRST, pp0[..^pp1.Length], pp1) : string.Format(FORMAT_OMIT_SECOND, pp0, pp1));

                return true;
            }

            char lastLetter = str[^1];
            if (lastLetter >= '가' && lastLetter <= '힣')
            {
                int lastConsonantIndex = (lastLetter - '가') % 28;
                if (lastConsonantIndex == 0)
                {
                    formattingInfo.Write(pp1);
                }
                else
                {
                    formattingInfo.Write(pp0);
                }
            }
            else
            {
                formattingInfo.Write(pp0.EndsWith(pp1) ? string.Format(FORMAT_OMIT_FIRST, pp0[..^pp1.Length], pp1) : string.Format(FORMAT_OMIT_SECOND, pp0, pp1));
            }

            return true;
        }
    }
}
