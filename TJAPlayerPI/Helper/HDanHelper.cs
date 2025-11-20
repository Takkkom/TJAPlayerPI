using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJAPlayerPI.Helper
{
    public static class HDanHelper
    {
        /// <summary>
        /// n個の条件で段位認定モードのステータスを返します。
        /// </summary>
        /// <param name="dan_C">条件。</param>
        /// <returns>ExamStatus。</returns>
        public static Exam.Status GetExamStatus(Dan_C[] dan_C, Dan_C Gauge)
        {
            var status = Exam.Status.Better_Success;
            var count = 0;
            for (int i = 0; i < 3; i++)
            {
                if (dan_C[i] is not null && dan_C[i].IsEnable == true)
                    count++;
            }
            for (int i = 0; i < count; i++)
            {
                if (!dan_C[i].GetCleared(true)) status = Exam.Status.Success;
            }
            if (Gauge.IsEnable)
                if (!Gauge.GetCleared(true))
                    status = Exam.Status.Success;
            for (int i = 0; i < count; i++)
            {
                if (!dan_C[i].GetCleared(false)) status = Exam.Status.Failure;
            }
            if (Gauge.IsEnable)
                if (!Gauge.GetCleared(false))
                    status = Exam.Status.Failure;
            return status;
        }
    }
}
