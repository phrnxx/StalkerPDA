using System;
using System.Text;

namespace StalkerPDA.Services
{
    public class ProceduralALife
    {
        private Random _rng = new Random();

        private string[] _b1_Prefix = {
            "Увага всім.",
            "Прийом на частоті.",
            "Хто поруч?",
            "Ледь живий лишився...",
            "Є тут хто живий?"
        };

        private string[] _b2_Context = {
            "Тільки-но туман впав,",
            "Прямо посеред ночі,",
            "Поки шукав схованку,",
            "Йшов повз аномалії і",
            "Щойно викид закінчився,"
        };

        private string[] _b3_Action = {
            "помітив",
            "зустрів",
            "ледь обійшов",
            "підстрелив",
            "побачив у бінокль"
        };

        private string[] _b4_Adjective = {
            "величезного",
            "дуже підозрілого",
            "пораненого",
            "озброєного до зубів",
            "абсолютно невідомого"
        };

        private string[] _b5_Subject = {
            "бандита",
            "кровососа",
            "найманця",
            "монолітівця",
            "мутанта"
        };

        private string[] _b6_Location = {
            "біля старого мосту на Кордоні",
            "у центрі Смітника",
            "на заводі Росток",
            "неподалік від Агропрому",
            "на краю Темної Долини"
        };

        private string[] _b7_Detail = {
            "із новеньким 'Вінторізом'.",
            "у повністю розірваному комбезі.",
            "з контейнером під артефакти.",
            "який тягнув чийсь рюкзак.",
            "без розпізнавальних знаків."
        };

        private string[] _b8_Result = {
            "Довелося залягти в кущі.",
            "Відкрив вогонь на ураження.",
            "Зробив вигляд, що мене там немає.",
            "Втратив половину патронів.",
            "Здобув непоганий трофей."
        };

        private string[] _b9_Request = {
            "Терміново потрібні бинти.",
            "Шукаю покупця на хабар.",
            "Хто допоможе зачистити територію?",
            "Є в когось зайві антиради?",
            "Можу поділитися інфою за консерви."
        };

        private string[] _b10_Postfix = {
            "Кінець зв'язку.",
            "Будьте обережні.",
            "Чекаю в безпечному місці.",
            "Оглядайтесь частіше.",
            "Хай береже вас Зона."
        };

        public string GenerateMessage()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(_b1_Prefix[_rng.Next(_b1_Prefix.Length)]).Append(" ");
            sb.Append(_b2_Context[_rng.Next(_b2_Context.Length)]).Append(" ");
            sb.Append(_b3_Action[_rng.Next(_b3_Action.Length)]).Append(" ");
            sb.Append(_b4_Adjective[_rng.Next(_b4_Adjective.Length)]).Append(" ");
            sb.Append(_b5_Subject[_rng.Next(_b5_Subject.Length)]).Append(" ");
            sb.Append(_b6_Location[_rng.Next(_b6_Location.Length)]).Append(" ");
            sb.Append(_b7_Detail[_rng.Next(_b7_Detail.Length)]).Append(" ");
            sb.Append(_b8_Result[_rng.Next(_b8_Result.Length)]).Append(" ");
            sb.Append(_b9_Request[_rng.Next(_b9_Request.Length)]).Append(" ");
            sb.Append(_b10_Postfix[_rng.Next(_b10_Postfix.Length)]);

            return sb.ToString();
        }
    }
}