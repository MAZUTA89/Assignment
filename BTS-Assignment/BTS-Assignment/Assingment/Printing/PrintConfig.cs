using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS_Assignment.Printing
{
    public static class PrintConfig
    {
        // Константы, соответствующие тегам из документа
        public const string НомерПоручения = "НомерПоручения";
        public const string НомерДоговора = "НомерДоговора";
        public const string ДатаДоговора = "ДатаДоговора";
        public const string Место = "Место";
        public const string ТекущаяДата = "ТекущаяДата";
        public const string Загрузка = "Загрузка";
        public const string Транспорт = "Транспорт";
        public const string Водитель = "Водитель";
        public const string Маршрут = "МаршрутРУ";
        public const string Товар = "Товар";
        public const string Отправление = "Отправление";
        public const string ПунктВвоза = "ПунктВвоза";
        public const string Назначение = "Назначение";
        public const string Получатель = "Получатель";
        public const string Разгрузка = "Разгрузка";
        public const string Должность = "ДолжностьСпец";
        public const string ФИО = "ФИОСпец";
        public const string Доверенность = "ДоверенностьСпец";
        public const string Фирма = "Фирма";
        public const string УНП = "УНП";
        public const string АдресФирмы = "АдресФирмы";
        public const string ДолжностьПеревозчика = "ДолжностьПеревозчика";
        public const string ФИОПеревозчика = "ФИОПеревозчика";

        public static readonly Dictionary<string, object> Tags;

        static PrintConfig()
        {
                Tags = new Dictionary<string, object>
            {
                { НомерПоручения, НомерПоручения },
                { НомерДоговора, НомерДоговора },
                { ДатаДоговора, ДатаДоговора },
                { Место, Место },
                { ТекущаяДата, ТекущаяДата },
                { Загрузка, Загрузка },
                { Транспорт, Транспорт },
                { Водитель, Водитель },
                { Маршрут, Маршрут },
                { Товар, Товар },
                { Отправление, Отправление },
                { ПунктВвоза, ПунктВвоза },
                { Назначение, Назначение },
                { Получатель, Получатель },
                { Разгрузка, Разгрузка },
                { Должность, Должность },
                { ФИО, ФИО },
                { Доверенность, Доверенность },
                { Фирма, Фирма },
                { УНП, УНП },
                { АдресФирмы, АдресФирмы },
                { ДолжностьПеревозчика, ДолжностьПеревозчика },
                { ФИОПеревозчика, ФИОПеревозчика }
            };
        }
    }
}
