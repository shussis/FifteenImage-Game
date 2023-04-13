#region Using

using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Globalization;
using System.Threading;

#endregion
namespace FifteenImage
{
    //����� ����������� ������� ���������� IDA*  
        internal sealed class IDA : SearchBase
        {
            #region Constructor
            public IDA(int[] start)
            {
            //��������� ���������
            _start = start;
            }
            #endregion
            #region Variables
            //������ ����������� ��������� ���������, ���������� ������ 
            //������������ ������ ����, ����� �� �����
            public int[] _start;
            //���������� �������������� ���-�� ������������ ���������
            public static ulong CountStates;
            //���������� ������� ������� ������ 
            public static int DepthDistributeStep;
            //���������� ����� �������
            readonly DateTime _timeStart = DateTime.Now;
            #endregion
            #region Classes
            //��������� ����������� ����������� (��������� ���������)
            struct Move
            {
                public int m_x, m_y;
                public Move(int x, int y)
                {
                    m_x = x;
                    m_y = y;
                }
            }
            //����� ����������� ��������� ������� , 
            //� ������ ������ ������� ����� ���� 15 
            class State
            {
                //������� �������� �����
                public uint[,] m_cells;
                //������� ��������
                public int m_emptyX, m_emptyY;
                //����������� ������� �������� �������� �� ������ ��������� �������
                public bool IsFinal(int x, int y, int value)
                {
                    return x == -1 ||
                        m_cells[x, y] == value &&
                        IsFinal(y == 0 ? (x - 1) : x, y == 0 ?
                        Dimension - 1 : (y - 1), --value);
                }
                //�������� �������� �� ������ ��������� �������
                public bool IsFinalPosition()
                {
                    return IsFinal(Dimension - 1, Dimension - 2, Dimension * Dimension - 2);
                }
                //������� ��������������� �������� ������ �� � �����������
                public void SetCell(int x, int y, int value)
                {
                    m_cells[x, y] = (uint)(value - 1);
                }
                //������� ��������������� ��������� ��������
                public void SetEmpty(int x, int y)
                {
                    m_emptyX = x;
                    m_emptyY = y;
                }
                //������� �����������
                public uint[,] Move(Move move)
                {
                    m_cells[m_emptyX, m_emptyY] = m_cells[move.m_x, move.m_y];
                    m_cells[move.m_x, move.m_y] = (uint)(Dimension * Dimension);
                    m_emptyX = move.m_x;
                    m_emptyY = move.m_y;
                    return m_cells;
                }
                //�����������
                public State()
                {
                    m_cells = new uint[Dimension, Dimension];
                }
            }
           class SearchIDA
            {
                //���������� ��� �������� ������� ���������
                public State _state = new State();
                //������ ����������� �� ���������� ��������� � ������� , 
                //�.�. ������� ������� ��������� �����
                public List<Move> _solution = new List<Move>();
                //���������� ������� ������������ ��� ������ �� ���������� ���������� 
                //���� ���������� ������� �� ������� ����� �� ��� �� ���� 
                //�� ����� �������� ������ ����������� 
                int _nStepDepth;
                #region Functions
                /// <summary>
                /// �������� ������� ������ ������������ ����
                /// </summary>
                /// <returns>List<Move> ��������� ������ ������������� ������������ ����</returns>
                public List<Move> GetOptimalSolution()
                {
                    DepthDistributeStep = 0;
                    if (Dimension - 1 == _state.m_emptyX &&
                        _state.m_emptyX == _state.m_emptyY &&
                        _state.IsFinalPosition())//���� ��������� ��������� �������� �����
                        return new List<Move>();//�� ����� ������ ������ ����� ����
                    //������������ ��� ��������� ��������� � ����� ���������� ��������� , 
                    //� ���������� ������� ������ ��� ������ �� ���������� ��������� 
                    _nStepDepth = 0;
                    int depthDistributeStep = 0;
                    //����� ����� 
                    if (!DistributeStep(_state.m_emptyX, _state.m_emptyY, 0, 0, true, depthDistributeStep))
                        //�� ������ � ��������� ���������  , ����� ����� ������ � 
                        //������� ������������� ������� _nStepDepth + 1 
                        while (!DistributeStep(_state.m_emptyX,
                            _state.m_emptyY, 0, 0, false, depthDistributeStep))
                            //����� ����������� ������� ���� �� ������ �� ���������� ����������
                            _nStepDepth++;
                    return _solution;
                }
                /// <summary>
                /// ������������ ��������� ���� , � ������ ���� 15 �� 
                /// ����� ������ ��������� �����������
                /// ������� ��������� ��� ����������� ������� ������
                ///   0 1 2 3
                ///   - - - - Y
                /// 0|
                /// 1|
                /// 2|
                /// 3|
                ///  X
                /// </summary>
                /// <param name="emptyX">��������� �������� �� ��� X</param>
                /// <param name="emptyY">��������� �������� �� ��� Y</param>
                /// <param name="emptyXOffset">����������� �������� ����� ��� X</param>
                /// <param name="emptyYOffset">����������� �������� ����� ��� Y</param>
                /// <param name="edge">false - �������� ����� � ��������� ��������</param>
                /// <returns>true - ���������� ��� �������� ���� � �������  , false �� ��������</returns>
                bool DistributeStep(int emptyX, int emptyY,
                    int emptyXOffset, int emptyYOffset, bool edge, int depthDistributeStep)
                {
                    depthDistributeStep++;
                    if (depthDistributeStep > DepthDistributeStep)
                        DepthDistributeStep = depthDistributeStep;
                    //� ���� "15" �������� 4 �������� ����������� ����� �� ������ �����
                    //���� ����������� �������� ������ �������� 
                    if (emptyY != Dimension - 1 &&
                             -1 != emptyYOffset && //� ���� �� �� ������ ���������� �����������
                             DoMakeStep(emptyX, emptyY, 0, 1, edge, depthDistributeStep)) //�� ������� ��� 
                    {
                        //������ ����������� ������� � ���� ��� �������� 
                        //������������ ������� � ���� , 
                        //�.�. �������� ����� �� ����� ���������� ���� , ������� 
                        //���� ��� � ������ ������  
                        _solution.Add(new Move(emptyX, emptyY + 1));
                        return true;
                    }
                    if (emptyY != 0 && //���� ����������� �������� ����� �������� 
                        1 != emptyYOffset && //� ���� �� �� ������ ���������� �����������
                        DoMakeStep(emptyX, emptyY, 0, -1, edge, depthDistributeStep))//�� ������� ���
                    {
                        _solution.Add(new Move(emptyX, emptyY - 1));
                        return true;
                    }
                    if (emptyX != Dimension - 1 && //���� ����������� �������� ���� �������� 
                        -1 != emptyXOffset && //� ���� �� �� ������ ���������� �����������
                        DoMakeStep(emptyX, emptyY, 1, 0, edge, depthDistributeStep))//�� ������� ���
                    {
                        _solution.Add(new Move(emptyX + 1, emptyY));
                        return true;
                    }
                    if (emptyX != 0 && //���� ����������� �������� ����� �������� 
                        1 != emptyXOffset && //� ���� �� �� ������ ���������� �����������
                        DoMakeStep(emptyX, emptyY, -1, 0, edge, depthDistributeStep))//�� ������� ���
                    {
                        _solution.Add(new Move(emptyX - 1, emptyY));
                        return true;
                    }
                    return false;
                }
                /// <summary>
                /// ��������� �� �������� �� ��������� ������ ������ �����������
                /// </summary>
                /// <param name="cell">���������� ������ � ����� ��������� ��������</param>
                /// <param name="emptyX">��������� �������� �� ��� X</param>
                /// <param name="emptyY">��������� �������� �� ��� Y</param>
                /// <param name="emptyXOffset">����������� �������� ����� ��� X</param>
                /// <param name="emptyYOffset">����������� �������� ����� ��� Y</param>
                /// <returns></returns>
                static bool IsGoodMove(uint cell //���������� ������ � ����� ��������� ��������
                    , int emptyX, int emptyY, int emptyXOffset, int emptyYOffset)
                {

                    if (emptyXOffset != 0) //�� ���������� ����������� �� ��� X
                    {
                        int posMustBe = (int)(cell / Dimension);//������� ������� ������
                        //������ ����� ��� ������� ����� ��� ����������� �� ����� 
                        //�������� (��� ��� ����������� �������� �� ����� ������)
                        if (posMustBe == emptyX)
                            return true;//��������� ������
                        if (emptyX < posMustBe)//���� ������� ������� ������ ��������� ������
                            //�� ����� ������ ���� �� ���������� ����������� 
                            //�������� ���� (=> ������ �����) , �.�. �� ��������� � ����
                            return emptyXOffset < 0;
                        return emptyXOffset > 0;
                    }
                    else //�� ���������� ����������� �� ��� Y
                    {
                        int posMustBe = (int)(cell % Dimension);//������� ������� ������
                        //������ ����� ��� ������� ����� ��� ����������� �� ����� �������� 
                        //(��� ��� ����������� �������� �� ����� ������)
                        if (posMustBe == emptyY)
                            return true;//��������� ������
                        if (emptyY < posMustBe)//���� ������� ������� ������ ��������� ������
                            //�� ����� ������ ���� �� ���������� ����������� �������� ����� 
                            //(=> ������ ������) , �.�. �� ��������� � ����
                            return emptyYOffset < 0;
                        return emptyYOffset > 0;
                    }
                }
                /// <summary>
                /// ��������� ���
                /// </summary>
                /// <param name="emptyX">��������� �������� �� ��� X</param>
                /// <param name="emptyY">��������� �������� �� ��� Y</param>
                /// <param name="emptyXOffset">����������� �������� ����� ��� X</param>
                /// <param name="emptyYOffset">����������� �������� ����� ��� Y</param>
                /// <param name="edge">false - �������� ����� � ��������� ��������</param>
                /// <returns>true - ��� �������� ���� � �������  , false �� ��������</returns>
                bool DoMakeStep(int emptyX, int emptyY, int emptyXOffset,
                    int emptyYOffset, bool edge, int depthDistributeStep)
                {
                    //����� ��������� ��������
                    int newEmptyX = emptyX + emptyXOffset;
                    int newEmptyY = emptyY + emptyYOffset;
                    //���������� ������ � ����� ��������� ��������
                    uint cell = _state.m_cells[newEmptyX, newEmptyY];
                    //�������� �� ������ ����������� �������� ��������� ������
                    bool isGoodMove = IsGoodMove(cell, emptyX, emptyY, emptyXOffset, emptyYOffset);
                    if (isGoodMove)
                    {
                        CountStates++;
                        //���� ����������� �������� �� �������� ��������� ������ �� ���������� �����������
                        _state.m_cells[emptyX, emptyY] = cell;
                        if (edge &&//�� ��������� � ��������� ������ � ������������� ����������� 
                            Dimension - 1 == newEmptyX &&//���� �������� �� ����� �� ��� X
                            Dimension - 1 == newEmptyY &&//���� �������� �� ����� �� ��� Y
                            _state.IsFinalPosition())//���� ����� ��������� ������ �������� � ���� 
                        {
                            //�� ��������� true - ���� �������� 
                            return true;
                        }
                        //���� ����������� �������� �������� ��������� ������ ,
                        //�� ��������� �������� ����������� ��� ������ ��������� ��������
                        if (DistributeStep(newEmptyX, newEmptyY, emptyXOffset,
                            emptyYOffset, edge, depthDistributeStep))
                        {
                            //������ ����������� ������� � ���� ��� �������� ������������ ������� ���� 
                            //� ���� , �.�. �������� ����� �� ����� ���������� ���� , ����� ������  
                            return true;
                        }
                        //���� ��� 4 ��������� ����������� �������� ��������� �� ���������� ����� 
                        //��������� �������� � ������� � false
                        _state.m_cells[newEmptyX, newEmptyY] = cell;
                        return false;
                    }
                    //��������� �� �� � ��������� ������ ������ �� ���������� ���������� 
                    if (!edge)
                    {
                        CountStates++;
                        //������ ����������� �������� ,�� �� ��������� ��� 
                        //�������� ������� ����������� 
                        //������� ������ ������ �����������
                        _state.m_cells[emptyX, emptyY] = cell;
                        --_nStepDepth;//��������� ����� ����� ����������� , 
                        //� ������ ,���� �� �� ����������� ,
                        //�� ����� ������� ��� ��������� ����������� ��� ������ 
                        //�� ���������� ���������� 
                        if (_nStepDepth >= 0
                            ? DistributeStep(newEmptyX, newEmptyY, emptyXOffset,
                                emptyYOffset, false, depthDistributeStep)
                            : DistributeStep(newEmptyX, newEmptyY, emptyXOffset,
                                emptyYOffset, true, depthDistributeStep))
                        {
                            //������ ����������� ������� � ���� ��� �������� 
                            //������������ ������� ���� 
                            //� ���� , �.�. �������� ����� �� ����� 
                            //���������� ���� , ����� ������  
                            return true;
                        }
                        //��������� ������� ������ ������ �� ���������� ���������� 
                        //�� ���� ���������� , �������� ������� 
                        ++_nStepDepth;
                        //���������� ����� ��������� �������� � ������� � false
                        _state.m_cells[newEmptyX, newEmptyY] = cell;
                        return false;
                    }
                    return false;
                }
                #endregion
            }
            #endregion
            #region Functions
            //������ 0.2 ��� ������� ������� ���������� ������ ���������
            protected override void ShowStatistics()
            {
                while (Thread != null && Thread.IsAlive)//(arResult == null)
                {
                    TimeSpan timeSearch = DateTime.Now - _timeStart;
                    Progressing(
                        "������� ���-�� ������������� ���������\t: " + CountStates.ToString(CultureInfo.InvariantCulture) +
                        Environment.NewLine + "����� ������\t\t\t\t: " + timeSearch +
                        Environment.NewLine + "������� ������������ ������� ������ \t: " + DepthDistributeStep.ToString(CultureInfo.InvariantCulture));
                    Thread.Sleep(200);
            }
            }
            //�������� ����� �� ������ ��������� ��������� ���� ���������� � ��������
            //������ �������� ���-�� ����������� � ������� ����� �������
            public static bool DoHaveResolve(int[] ar)
            {
                int ch = 0;
                for (int i = 0; i < ar.Length; i++)
                    for (int j = 0; j < ar.Length; j++)
                    {
                        //�������� ����� ��� �������� �����������
                        if (ar[i] == ar.Length - 1 || ar[j] == ar.Length - 1)
                            continue;
                        if (i < j && ar[i] > ar[j])
                            ch++;
                    }
                //���� ������� ������� ������ , ���� ������ ��������� �����
                if (Dimension % 2 == 0)
                    for (int i = 0; i < ar.Length; i++)
                        if (ar[i] == ar.Length - 1)
                        {
                            int row = 0;
                            for (int h = i; h >= 0; h -= Dimension) row++;
                            if (row % 2 == 1)
                                ch++;
                        }
                return (ch % 2) == 0;
            }
            //�������� ������� ����������� ����� 
            protected override void StartSearch()
            {
                Started();
                {
                    SearchIDA searchIDA = new SearchIDA();
                    State state = new State();
                    for (int i = 0; i < Dimension * Dimension; ++i)
                    {
                        int value = _start[i];
                        value++;
                        if (value != Dimension * Dimension)
                            state.SetCell(i / Dimension, i % Dimension, value);
                        else
                            state.SetEmpty(i / Dimension, i % Dimension);
                    }
                    searchIDA._state.m_cells = (uint[,])state.m_cells.Clone();
                    searchIDA._state.m_emptyX = state.m_emptyX;
                    searchIDA._state.m_emptyY = state.m_emptyY;
                    CountStates = 0;
                    //����� ����� ������������ ����
                    List<Move> solution = searchIDA.GetOptimalSolution();
                    //�� �������� ����� ����������� �� ������� ������� � ���������
                    solution.Reverse();
                    ArResult = new int[solution.Count + 1][];
                    ArResult[0] = _start;
                    int arIndex = 1;
                    foreach (Move m in solution)
                    {
                        state.Move(m);
                        ArResult[arIndex] = new int[Dimension * Dimension];
                        int j = 0;
                        for (int ii = 0; ii < Dimension; ii++)
                            for (int jj = 0; jj < Dimension; jj++, j++)
                            {
                                if (ii == state.m_emptyX &&
                                    jj == state.m_emptyY)
                                    ArResult[arIndex][j] = Dimension * Dimension - 1;
                                else
                                    ArResult[arIndex][j] = (int)state.m_cells[ii, jj];// - 1;
                            }
                        arIndex++;
                }
                    TimeSpan timeSearch = DateTime.Now - _timeStart;
                    if (ArResult.Length - 1 != 0)
                    {
                        int v=(ArResult.Length - 1)%10;
                        string st = (v == 2 || v == 3 || v == 4) ? " ����" : ((v == 1) ? " ���" : " �����");
                        Progressing("����",
                            "�����. " + Environment.NewLine + (ArResult.Length).ToString(CultureInfo.InvariantCulture) +
                            st+Environment.NewLine + "�����:" + timeSearch +
                            Environment.NewLine + "\n����� ���-�� ������������� ���������\t: " +
                            CountStates.ToString(CultureInfo.InvariantCulture) +
                            Environment.NewLine + "\n**** ����� �������� ****" +
                            Environment.NewLine + "\n����� ������\t: " + timeSearch +
                            Environment.NewLine + "\n���-�� ����� ��� ����� �������\t: " +
                            (ArResult.Length ).ToString(CultureInfo.InvariantCulture));
                    Global.globalCountStates = Convert.ToInt32(CountStates);
                    //Global.globalCountStates = Convert.ToInt32(CountStates);
                    //Global.globalmoves = Convert.ToInt32(ArResult.Length);
                }
                    else
                        Progressing("� ��� � ������ ������ ?");
                }
            Finished();
            }
            #endregion
        }

     
}