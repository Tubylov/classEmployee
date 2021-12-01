using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Komos
{
    class Program
    {

        public static void Rec(string message) // Обработчик события "Принять на работу"
        {
            Console.WriteLine(message);
        }
        public static void Dis(string message) // Обработчик события "Уволить"
        {
            Console.WriteLine(message);
        }

        static void Main(string[] args)
        {
            // 1 
            Employee Sergey = new Employee("Михайлов", "Сергей", "Владимирович");
            Employee Ivan = new Employee("Иванов");
            Sergey.Info();
            Console.WriteLine("\n");
            Ivan.Info();

            // 2-4
            Sergey.Gen = Employee.Gender.мужской;
            Sergey.Birthday = new DateTime(2007, 05, 13);
            Console.WriteLine("Сообщение:");
            Sergey.Info("Возраст");

            // 5
            List<Employee> employees = new List<Employee>(3);
            employees.Add(new Employee("Иванов", "Иван", "Иванович"));
            employees.Add(new Employee("Петров", "Петр"));
            employees.Add(new Employee("Сидоров", "Максим", "Викторович"));

            int d, m, y, age;

            foreach (Employee i in employees)
            {
                Console.Write("Введите день: ");
                d = int.Parse(Console.ReadLine());
                Console.Write("Введите месяц: ");
                m = int.Parse(Console.ReadLine());
                Console.Write("Введите год: ");
                y = int.Parse(Console.ReadLine());
                i.Birthday = new DateTime(y, m, d);
            }
            
            foreach (Employee i in employees)
            {
                Console.WriteLine(i.Birthday);
                Console.WriteLine(i.Age);
            }

            Console.Write("Введите возраст: ");
            age = int.Parse(Console.ReadLine());
            Employee.createEmployeeFileByAge(employees, age);

            // 6
            Sergey.Birthday = new DateTime(1991, 8, 23);
            Sergey.OnRecruit += Rec;
            Sergey.OnDismiss += Dis;
            Sergey.Dismiss(Sergey);
        }
    }

    delegate void EmployeeHandler(string message);

    // 1.Публичный класс «Работник»

    class Employee
    {
        string name;
        string surname;
        string patronymic;
        Gender gender;
        DateTime birthday;
        int age = 1;

        public enum Gender : byte // перечисление для поля "пол работника"
        {
            мужской,
            женский
        }


        // Конструктор класса (Фамилия - обязательный параметр, Имя и Отчество - необязательный)

        public Employee(string surname, string name = "", string patronymic = "")
        {
            this.surname = surname;
            this.name = name;
            this.patronymic = patronymic;
        }


        // 2.Свойства

        public string Name { get; set; }
        public string Patronymic { get; set; }
        public Gender Gen { get; set; }
        public string Surname {
            get
            {
                return surname;
            }
            set
            {
                if(value == "" || value == null)
                {
                    Console.WriteLine("Фамилия не записана!");
                }
                else
                {
                    surname = value;
                }
            }
        }
        public int Age {
            get
            {
                return age;
            }
        }
        public string FullName
        {
            get
            {
                surname = surname.Substring(0, 1).ToUpper() + surname.Remove(0, 1).ToLower();

                if(name == "" || patronymic == "")
                {
                    Console.WriteLine($"Невозможно получить ФИО у {surname}. Имя или Отчество не заполнено");
                    return surname + " " + name;
                }
                else
                {
                    
                    return surname + " " + char.ToUpper(name[0]) + "." + char.ToUpper(patronymic[0]) + ".";
                }
                
            }
        }
        public DateTime Birthday
        {
            get
            {
                return birthday;
            }
            set
            {
                age = GetAge(value);
                birthday = value;
            }
        }


        // 3.Публичный статический метод, который вычисляет
        //   возраст по переданной параметром дате рождения.

        public static int GetAge(DateTime datе)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - datе.Year;
            if(datе.AddYears(age) > today)
            {
                age--;
            }
            return age;
        }


        // 4.Публичный метод, который отображает всю информацию
        //   о сотруднике в диалоге типа «Сообщение»

        public void Info()
        {
            string birth;
            if(birthday.ToString("dd.MM.yyyy") == "01.01.0001")
            {
                birth = "не указана";
            }
            else
            {
                birth = birthday.ToString("dd.MM.yyyy");
            }
            Console.WriteLine(
                "ФИО: {0}\nДата рождения: {1}\nПол: {2}",
                FullName,
                birth,
                gender
            );
        }

        // Перегрузка созданного метода для дополнительного вывода возраста,
        // который на вход принимает строку

        public void Info(string add_age)
        {
            string birth;
            if (birthday.ToString("dd.MM.yyyy") == "01.01.0001")
            {
                birth = "не указана";
            }
            else
            {
                birth = birthday.ToString("dd.MM.yyyy");
            }
            Console.WriteLine(
                "ФИО: {0}\nДата рождения: {1}\nПол: {2}\nВозраст: {3}",
                FullName,
                birth,
                gender,
                age
            );
        }


        // 5.Сформировать файл во временном каталоге

        public static void createEmployeeFileByAge(List<Employee> employees, int age)
        {
            try
            {
                DateTime currentData = DateTime.Now;
                string fileName = age + "_" + currentData.ToString("dd.MM.yyyy HH:mm:ss") + ".tmp";
                string path = Path.GetTempPath();
                string filePath = Path.Combine(path, fileName);

                using (StreamWriter sw = new StreamWriter(filePath, false, System.Text.Encoding.Default))
                {
                    foreach (Employee i in employees)
                    {
                        if (i.Age == age)
                        {
                            sw.WriteLine(i.FullName);
                        }
                    }
                }
                Console.WriteLine($"Файл сформирован по данному пути: {filePath}");
            }
            catch
            {
                Console.WriteLine("Непредвиденная ошибка");
            }
        }


        // 6.События

        public event EmployeeHandler OnRecruit; // событие "Принять на работу"
        public event EmployeeHandler OnDismiss; // событие "Уволить"

        public void Recruit(Employee emp)
        {
            if(emp.age > 18)
            {
                OnRecruit?.Invoke("Вы приняты на работу!");
            }
            else
            {
                OnRecruit?.Invoke("Невозможно принять на работу, так как ему меньше 18 лет");
            }
        }

        public void Dismiss(Employee emp)
        {
            OnDismiss?.Invoke("Вы уволены!");
        }
    }
}