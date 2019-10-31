using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Gloson.Data.Oracle {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Oracle Long Process
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class OracleLongOperation : IDisposable {
    #region Private Data

    private int m_CurrentStep;

    private int m_Index = -1;
    private int m_Id    = -1;

    #endregion Private Data

    #region Algorithm

    private void CoreStartLongProcess() {
      using (var q = Connection.CreateCommand()) {
        q.CommandText =
          @"declare
              rIndex BINARY_INTEGER;
              Slno   BINARY_INTEGER;
            begin 
              rIndex := dbms_Application_Info.Set_Session_LongOps_NoHint;
          
              dbms_Application_Info.Set_Session_LongOps(
                 rIndex,
                 Slno,
                :prm_Title,
                 null,
                 null,
                :prm_CurrentStep,
                :prm_FinalStep,
                 null
              );
          
             :rIndex := rIndex;
             :Slno := Slno;
            end;";

        var prmTitle = q.CreateParameter();

        prmTitle.ParameterName = ":prm_Title";
        prmTitle.Direction = ParameterDirection.Input;
        prmTitle.DbType = DbType.String;
        prmTitle.Value = Title;

        var prmCurrentStep = q.CreateParameter();

        prmCurrentStep.ParameterName = ":prm_CurrentStep";
        prmCurrentStep.Direction = ParameterDirection.Input;
        prmCurrentStep.DbType = DbType.Int32;
        prmCurrentStep.Value = CurrentStep;

        var prmFinalStep = q.CreateParameter();

        prmFinalStep.ParameterName = ":prm_FinalStep";
        prmFinalStep.Direction = ParameterDirection.Input;
        prmFinalStep.DbType = DbType.Int32;
        prmFinalStep.Value = CurrentStep;

        //---

        var prmrIndex = q.CreateParameter();

        prmrIndex.ParameterName = ":rIndex";
        prmrIndex.Direction = ParameterDirection.Output;
        prmrIndex.DbType = DbType.Int32;

        var prmrSlno = q.CreateParameter();

        prmrSlno.ParameterName = ":Slno";
        prmrSlno.Direction = ParameterDirection.Output;
        prmrSlno.DbType = DbType.Int32;

        q.Parameters.Add(prmTitle);
        q.Parameters.Add(prmCurrentStep);
        q.Parameters.Add(prmFinalStep);
        q.Parameters.Add(prmrIndex);
        q.Parameters.Add(prmrSlno);
        
        q.ExecuteNonQuery();

        m_Index = Convert.ToInt32(prmrIndex.Value);
        m_Id = Convert.ToInt32(prmrSlno.Value);
      }
    }

    private void CoreNextStep() {
      using (var q = Connection.CreateCommand()) {
        q.CommandText =
         @"begin
            dbms_Application_Info.Set_Session_LongOps(
              :rIndex,
              :Slno,
              :prm_Title,
               null,
               null,
              :prm_CurrentStep,
              :prm_FinalStep,
               null);
           end;";

        var prmrIndex = q.CreateParameter();

        prmrIndex.ParameterName = ":rIndex";
        prmrIndex.Direction = ParameterDirection.Input;
        prmrIndex.DbType = DbType.Int32;
        prmrIndex.Value = m_Index;

        var prmrSlno = q.CreateParameter();

        prmrSlno.ParameterName = ":Slno";
        prmrSlno.Direction = ParameterDirection.Input;
        prmrSlno.DbType = DbType.Int32;
        prmrSlno.Value = m_Id;

        var prmTitle = q.CreateParameter();

        prmTitle.ParameterName = ":prm_Title";
        prmTitle.Direction = ParameterDirection.Input;
        prmTitle.DbType = DbType.String;
        prmTitle.Value = Title;

        var prmCurrentStep = q.CreateParameter();

        prmCurrentStep.ParameterName = ":prm_CurrentStep";
        prmCurrentStep.Direction = ParameterDirection.Input;
        prmCurrentStep.DbType = DbType.Int32;
        prmCurrentStep.Value = CurrentStep;

        var prmFinalStep = q.CreateParameter();

        prmFinalStep.ParameterName = ":prm_FinalStep";
        prmFinalStep.Direction = ParameterDirection.Input;
        prmFinalStep.DbType = DbType.Int32;
        prmFinalStep.Value = CurrentStep;

        q.Parameters.Add(prmrIndex);
        q.Parameters.Add(prmrSlno);
        q.Parameters.Add(prmTitle);
        q.Parameters.Add(prmCurrentStep);
        q.Parameters.Add(prmFinalStep);

        q.ExecuteNonQuery();
      }
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public OracleLongOperation(IDbConnection connection,
                               string title,
                               int finalStep,
                               int currentStep) {
      if (null == connection)
        throw new ArgumentNullException(nameof(connection));
      else if (connection.State != ConnectionState.Open ||
               connection.State != ConnectionState.Fetching ||
               connection.State != ConnectionState.Executing ||
               connection.State != ConnectionState.Connecting)
        throw new ArgumentException("Not connected", nameof(connection));

      if (finalStep < 1)
        throw new ArgumentOutOfRangeException(nameof(finalStep));
      else if (currentStep < 0)
        throw new ArgumentOutOfRangeException(nameof(currentStep));
      else if (currentStep > finalStep)
        throw new ArgumentOutOfRangeException(nameof(currentStep));

      Connection = connection;

      FinalStep = finalStep;
      m_CurrentStep = currentStep;

      Title = title?.Trim() ?? $"{Assembly.GetEntryAssembly().GetName().Name}";

      CoreStartLongProcess();
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public OracleLongOperation(IDbConnection connection,
                               string title,
                               int finalStep)
      : this(connection, title, finalStep, 0) { }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public OracleLongOperation(IDbConnection connection,
                               string title)
      : this(connection, title, 100, 0) { }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public OracleLongOperation(IDbConnection connection)
      : this(connection, null, 100, 0) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Connection
    /// </summary>
    public IDbConnection Connection { get; }

    /// <summary>
    /// Title
    /// </summary>
    public String Title { get; }

    /// <summary>
    /// Initial Step
    /// </summary>
    public int FinalStep { get; }

    /// <summary>
    /// Current Step
    /// </summary>
    public int CurrentStep {
      get {
        return m_CurrentStep;
      }
      set {
        if (m_CurrentStep == value)
          return;

        m_CurrentStep = value;

        CoreNextStep();
      }
    }

    /// <summary>
    /// Title
    /// </summary>
    public override string ToString() => Title;

    #endregion Public

    #region IDisposable

    /// <summary>
    /// Is Disposed
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Dispose
    /// </summary>
    private void Dispose(bool disposing) {
      if (IsDisposed)
        return;

      if (!disposing)
        return;

      try {
        CurrentStep = FinalStep;
      }
      finally {
        IsDisposed = true;
      }
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose() {
      Dispose(true);
    }

    #endregion IDisposable
  }
}
