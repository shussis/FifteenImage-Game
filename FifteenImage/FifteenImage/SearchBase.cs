#region Using

using System;
using System.Threading;

#endregion
namespace FifteenImage
{
    public class SearchEventArgs : EventArgs
        {
            public string Title;
            public string Mes;
            public bool DoAddMes;
            public SearchEventArgs(string title, string mes, bool doAddMes) { Title = title; Mes = mes; DoAddMes = doAddMes; }
            public SearchEventArgs() { }
            public SearchEventArgs(string mes) { Title = string.Empty; Mes = mes; DoAddMes = false;}
            public SearchEventArgs(string title, string mes) { Title = string.Empty; Mes = mes; DoAddMes = false; }
            public SearchEventArgs(string mes, bool doAddMes) { Title = string.Empty; Mes = mes; DoAddMes = doAddMes;  }
        }
        public abstract class SearchBase
        {
            //Размерность матрицы поиска
            public static int Dimension = 4;
            public static int Quantity = Dimension * Dimension;
            #region Events
            public event EventHandler<SearchEventArgs> AlgStarted;
            public event EventHandler<SearchEventArgs> AlgProgressing;
            public event EventHandler<SearchEventArgs> AlgFinished;
            protected void Started()
            {
                AlgStarted?.Invoke(this, new SearchEventArgs());
            }
            protected void Progressing(string mes, bool doAddMes)
            {
                AlgProgressing?.Invoke(this, new SearchEventArgs(mes, doAddMes));
            }
            protected void Progressing(string mes)
            {
                AlgProgressing?.Invoke(this, new SearchEventArgs(mes, false));
            }
            protected void Progressing(string title, string mes)
            {
                AlgProgressing?.Invoke(this, new SearchEventArgs(title, mes, false));
            }
            protected void Finished()
            {
                AlgFinished?.Invoke(this, new SearchEventArgs());
            }
            public bool EventStartAdded => AlgStarted != null;
            public bool EventProgressAdded { get { return AlgProgressing != null; } }
            public bool EventFinishAdded { get { return AlgFinished != null; } }
            public bool EventsAdded => AlgStarted != null && AlgProgressing != null && AlgFinished != null;

            #endregion
            #region Variables
            //результат в виде массива массивов шагов с начального до целевого
            public static int[][] ArResult;
            //указатель на поток в котором будет производится работа алгоритма
            protected static Thread Thread;
            //указатель на поток который будет выводить информацию о работе алгоритма
            protected static Thread ThreadStatistics;
            #endregion
            #region Functions
            //Функции остановки потоков поиска и вывода статистики
            public static string DoStop()
            {
                string mes = "";
                if (Thread != null)
                {
                    Thread.Abort();
                    ThreadStatistics.Abort();
                    if (!Thread.Join(1000))
                        mes = "Проблема остановки потока";
                    else
                    {
                        mes = "Поток успешно остановлен";
                        Thread = null;
                        ThreadStatistics = null;
                        
                    }
                }
                if (ThreadStatistics != null)
                {
                    ThreadStatistics.Abort();
                    if (!ThreadStatistics.Join(1000))
                        mes = "Проблема остановки потока";
                    else
                        ThreadStatistics = null;
                }
                return mes;
            }
            //Функция запускающая основной поток поиска и 
            //вспомогательный поток для вывода информации о 
            //работе алгоритма поиска
            public void DoStart()
            {
                try
                {
                    if (Thread != null)
                    {
                        if (!Thread.Join(10000))
                        {
                            Thread.Abort();
                            Thread.Join();
                        }
                        Thread = null;
                    }
                    ArResult = null;
                    Thread = new Thread(StartSearch);
                    Thread.IsBackground = true;
                    Thread.Start();
                    //Thread.Sleep(1000);
                    if (ThreadStatistics != null)
                    {
                        if (!ThreadStatistics.Join(10000))
                        {
                            ThreadStatistics.Abort();
                            ThreadStatistics.Join();
                        }
                        ThreadStatistics = null;
                    }
                    ThreadStatistics = null;
                    ThreadStatistics = new Thread(ShowStatistics) {IsBackground = true};
                    ThreadStatistics.Start();
                }
                catch (Exception ex)
                {
                    Progressing(ex.Message);
                    Finished();
                }
            }
            //Каждые 0.2 сек выводит текущую статистику работы алгоритма
            protected abstract void ShowStatistics();
            //Основная функция запускающая поиск 
            protected abstract void StartSearch();
            #endregion
        }

       
 }