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
            //����������� ������� ������
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
            //��������� � ���� ������� �������� ����� � ���������� �� ��������
            public static int[][] ArResult;
            //��������� �� ����� � ������� ����� ������������ ������ ���������
            protected static Thread Thread;
            //��������� �� ����� ������� ����� �������� ���������� � ������ ���������
            protected static Thread ThreadStatistics;
            #endregion
            #region Functions
            //������� ��������� ������� ������ � ������ ����������
            public static string DoStop()
            {
                string mes = "";
                if (Thread != null)
                {
                    Thread.Abort();
                    ThreadStatistics.Abort();
                    if (!Thread.Join(1000))
                        mes = "�������� ��������� ������";
                    else
                    {
                        mes = "����� ������� ����������";
                        Thread = null;
                        ThreadStatistics = null;
                        
                    }
                }
                if (ThreadStatistics != null)
                {
                    ThreadStatistics.Abort();
                    if (!ThreadStatistics.Join(1000))
                        mes = "�������� ��������� ������";
                    else
                        ThreadStatistics = null;
                }
                return mes;
            }
            //������� ����������� �������� ����� ������ � 
            //��������������� ����� ��� ������ ���������� � 
            //������ ��������� ������
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
            //������ 0.2 ��� ������� ������� ���������� ������ ���������
            protected abstract void ShowStatistics();
            //�������� ������� ����������� ����� 
            protected abstract void StartSearch();
            #endregion
        }

       
 }