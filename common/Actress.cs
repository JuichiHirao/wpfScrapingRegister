﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WpfScrapingRegister.dao;
using WpfScrapingRegister.data;

namespace WpfScrapingRegister.common
{
    class Actress
    {
        /// <summary>
        /// タグに設定された複数の女優名をParseして、複数の女優の配列としてリターン
        /// </summary>
        /// <returns></returns>
        public static string[] ParseTag(string myTag)
        {
            string[] arrData;
            if (myTag.IndexOf(",") < 0)
            {
                arrData = new string[1];
                arrData[0] = GetActressName(myTag);

                return arrData;
            }

            List<string> listActress = new List<string>();
            arrData = myTag.Split(',');
            foreach (string data in arrData)
            {
                // 「松ゆきの(2人目)」のような場合に括弧内を消す
                string act = GetActressName(data);
                if (act.Trim().Length > 0)
                    listActress.Add(act);
            }

            return listActress.ToArray();
        }

        public static string GetActressName(string myActressInfo)
        {
            string name = myActressInfo;

            if (myActressInfo.IndexOf("(") >= 0)
            {
                if (myActressInfo.IndexOf("(仮)") >= 0)
                    return name;

                Regex re = new Regex(String.Format("{0}.*{1}", Regex.Escape("("), ".*", Regex.Escape(")")));
                if (myActressInfo.IndexOf("(") >= 0)
                {
                    Match m = re.Match(myActressInfo);

                    if (m.Success)
                        name = myActressInfo.Replace(m.Groups[0].ToString(), "");
                }
            }
            return name;
        }

        public static List<string> AppendMatch(string myNames, List<string> myExistList)
        {
            List<string> listTarget = new List<string>();

            char splitChar = Actress.GetSplitChar(myNames);
            string[] arrLabel = myNames.Split(splitChar);

            foreach (string label in arrLabel)
                if (label.Length > 0)
                {
                    if (!myExistList.Exists(x => x == label))
                        listTarget.Add(label);
                }

            return listTarget;
        }

        public static char GetSplitChar(string myTag)
        {
            char splitChar = (char)0;

            if (myTag.IndexOf("／") >= 0)
                splitChar = '／';
            else if (myTag.IndexOf(",") >= 0)
                splitChar = ',';
            else if (myTag.IndexOf(" ") >= 0)
                splitChar = ' ';

            return splitChar;
        }

        public static bool IsNullChar(char myChar)
        {
            if ((int)myChar == 0)
                return false;

            return true;
        }

        internal static string GetEvaluation(string myTag, AvContentsDao contentsService, MySqlDbConnection dockerMysqlConn)
        {
            bool isFav = false;
            string[] arrActresses = common.Actress.ParseTag(myTag);
            string resultEvaluation = "", evaluation = "";
            int maxFav = 0;
            string maxActress = "";
            foreach (string actress in arrActresses)
            {
                string[] arrFavActress = contentsService.GetFavoriteActresses(actress, dockerMysqlConn);

                List<AvContentsData> avContentsList = new List<AvContentsData>();
                List<AvContentsData> avContentsFilenameLikeList = new List<AvContentsData>();
                if (arrFavActress.Length >= 1)
                {
                    isFav = true;
                    foreach (string favActress in arrFavActress)
                    {
                        List<AvContentsData> list = contentsService.GetActressList(actress, dockerMysqlConn);

                        foreach (AvContentsData data in list)
                        {
                            if (!avContentsList.Exists(x => x.Id == data.Id))
                                avContentsList.Add(data);
                        }
                    }
                }
                else
                {
                    avContentsList = contentsService.GetActressList(actress, dockerMysqlConn);
                    avContentsFilenameLikeList = contentsService.GetActressLikeFilenameList("%" + actress + "%", dockerMysqlConn, avContentsList);
                }

                int sumEvaluate = 0, unEvaluate = 0, maxEvaluate = 0;

                if (avContentsList.Count > 0)
                {
                    sumEvaluate = avContentsList.Sum(x => x.Rating);
                    unEvaluate = avContentsList.Where(x => x.Rating == 0).Count();
                    maxEvaluate = avContentsList.Max(x => x.Rating);
                }

                if (arrActresses.Length > 1)
                {
                    if (maxFav < maxEvaluate)
                    {
                        maxFav = maxEvaluate;
                        maxActress = actress;
                    }
                }

                if (sumEvaluate <= 0 || avContentsList.Count - unEvaluate <= 0)
                    evaluation = String.Format("全未評価 {0} ({1})", avContentsList.Count, avContentsFilenameLikeList.Count);
                else
                    evaluation = String.Format("未 {0}/全 {1} Max {2} Avg {3} ({4})", unEvaluate, avContentsList.Count, maxEvaluate, sumEvaluate / (avContentsList.Count - unEvaluate), avContentsFilenameLikeList.Count);

                if (arrActresses.Length > 1)
                    resultEvaluation = String.Format("{0} {1} {2}", resultEvaluation, actress, evaluation);
                else
                    resultEvaluation = evaluation;
            }

            if (arrActresses.Length > 1)
                resultEvaluation = String.Format("【{0} Max{1}】{2}", maxActress, maxFav, resultEvaluation);

            if (isFav)
                resultEvaluation = "Fav " + resultEvaluation;

            return resultEvaluation;
        }
    }
}
