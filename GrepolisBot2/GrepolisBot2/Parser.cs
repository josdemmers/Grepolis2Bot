using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace GrepolisBot2
{
    sealed class Parser
    {
        //Singleton
        private static readonly Parser m_Instance = new Parser();

//-->Constructor

        private Parser()
        {

        }

//-->Attributes

        //Singleton
        public static Parser Instance
        {
            get
            {
                return m_Instance;
            }
        }

//-->Methods

        public string fixSpecialCharacters(string p_String)
        {
            Regex l_Regex = new Regex(@"\\[uU]([0-9A-F]{4})", RegexOptions.IgnoreCase);
            string l_String = p_String;

            //Fix special characters
            l_String = l_Regex.Replace(l_String, l_Match => ((char)int.Parse(l_Match.Groups[1].Value, NumberStyles.HexNumber)).ToString());
            l_String = l_String.Replace("\\", "");

            return l_String;
        }

        public string decryptLink(string p_Link)
        {
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                string l_Key_str = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
                string l_Output = "";
                int i = 0;
                int l_Len = p_Link.Length;
                int l_Enc1; int l_Enc2; int l_Enc3; int l_Enc4;
                int l_Chr1; int l_Chr2; int l_Chr3;
                while (i < l_Len)
                {
                    l_Enc1 = l_Key_str.IndexOf(p_Link[i++]);
                    l_Enc2 = l_Key_str.IndexOf(p_Link[i++]);
                    l_Enc3 = l_Key_str.IndexOf(p_Link[i++]);
                    l_Enc4 = l_Key_str.IndexOf(p_Link[i++]);
                    l_Chr1 = (l_Enc1 << 2) | (l_Enc2 >> 4);
                    l_Chr2 = ((l_Enc2 & 15) << 4) | (l_Enc3 >> 2);
                    l_Chr3 = ((l_Enc3 & 3) << 6) | l_Enc4;
                    l_Output += Convert.ToChar(l_Chr1);
                    if (l_Enc3 != 64)
                    {
                        l_Output = l_Output + Convert.ToChar(l_Chr2);
                    }
                    if (l_Enc4 != 64)
                    {
                        l_Output = l_Output + Convert.ToChar(l_Chr3);
                    }
                }
                return l_Output;
            }
            catch (Exception)
            {
                l_IOHandler.debug("decryptLink failed: " + p_Link);
                return "false";
            }
        }

        public string unixToHumanTime(string p_Time)
        {
            Settings l_Settings = Settings.Instance;
            //double l_Offset = double.Parse(l_Settings.ServerTimeOffset) - double.Parse(l_Settings.LocaleTimeOffset);
            //double l_Offset = double.Parse(l_Settings.ServerTimeOffset);
            double l_Offset = double.Parse(l_Settings.LocaleTimeOffset);

            while (p_Time.Length > 10)
            {
                p_Time = p_Time.Remove(p_Time.Length - 1);
            }

            double l_Seconds = double.Parse(p_Time) + (l_Offset * 1.0);
            DateTime l_Time = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(l_Seconds);
            return l_Time.ToString();
        }

        public string createHtmlOverview2(Player p_Player)
        {
            //Note Modify this when adding new units (13/15)
            string l_Overview = "";
            string l_ActiveBuffs = "";
            string l_God = "";
            string l_TradeMode = "";
            string l_Wood = "";
            string l_Stone = "";
            string l_Iron = "";

            try
            {
                l_Overview = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">";
                l_Overview += "<html>";
                l_Overview += Environment.NewLine + "<head>";
                l_Overview += Environment.NewLine + "<title>Grepolis Town Overview</title>";
                //l_Overview += Environment.NewLine + "<link href=\"table.css\" rel=stylesheet type=\"text/css\">";
                l_Overview += Environment.NewLine + "<link href=\"css/custom.css\" rel=stylesheet type=\"text/css\">";
                l_Overview += Environment.NewLine + "<link href=\"css/theme.blue.css\" rel=stylesheet type=\"text/css\">";
                l_Overview += Environment.NewLine + "<script type=\"text/javascript\" src=\"js/jquery-1.11.2.js\"></script>";
                l_Overview += Environment.NewLine + "<script type=\"text/javascript\" src=\"js/jquery.tablesorter.js\"></script>";
                l_Overview += Environment.NewLine + "<script type=\"text/javascript\" src=\"js/jquery.tablesorter.widgets.js\"></script>";
                l_Overview += Environment.NewLine + "<META HTTP-EQUIV=\"Content-Type\" CONTENT=\"text/html; charset=UTF-8\">";
                l_Overview += Environment.NewLine + "</head>";
                //
                l_Overview += Environment.NewLine + "<script>";
                l_Overview += Environment.NewLine + "$(document).ready(function() {";
                //l_Overview += Environment.NewLine + "$('#table1').tablesorter({";
                l_Overview += Environment.NewLine + "$('#grepolis').tablesorter({";
                l_Overview += Environment.NewLine + "theme : 'blue',";
                l_Overview += Environment.NewLine + "widthFixed : true,";
                l_Overview += Environment.NewLine + "showProcessing: true,";
                l_Overview += Environment.NewLine + "headerTemplate : '{content} {icon}', // Add icon for various themes";
                l_Overview += Environment.NewLine + "widgets: [ 'zebra', 'stickyHeaders' , \"columns\"],";
                l_Overview += Environment.NewLine + "widgetOptions: {";
                l_Overview += Environment.NewLine + "// extra class name added to the sticky header row";
                l_Overview += Environment.NewLine + "stickyHeaders : '',";
                l_Overview += Environment.NewLine + "// number or jquery selector targeting the position:fixed element";
                l_Overview += Environment.NewLine + "stickyHeaders_offset : 0,";
                l_Overview += Environment.NewLine + "// added to table ID, if it exists";
                l_Overview += Environment.NewLine + "stickyHeaders_cloneId : '-sticky',";
                l_Overview += Environment.NewLine + "// trigger \"resize\" event on headers";
                l_Overview += Environment.NewLine + "stickyHeaders_addResizeEvent : true,";
                l_Overview += Environment.NewLine + "// if false and a caption exist, it won't be included in the sticky header";
                l_Overview += Environment.NewLine + "stickyHeaders_includeCaption : true,";
                l_Overview += Environment.NewLine + "// The zIndex of the stickyHeaders, allows the user to adjust this to their needs";
                l_Overview += Environment.NewLine + "stickyHeaders_zIndex : 2,";
                l_Overview += Environment.NewLine + "// jQuery selector or object to attach sticky header to";
                l_Overview += Environment.NewLine + "stickyHeaders_attachTo : null,";
                l_Overview += Environment.NewLine + "// jQuery selector or object to monitor horizontal scroll position (defaults: xScroll > attachTo > window)";
                l_Overview += Environment.NewLine + "stickyHeaders_xScroll : null,";
                l_Overview += Environment.NewLine + "// jQuery selector or object to monitor vertical scroll position (defaults: yScroll > attachTo > window)";
                l_Overview += Environment.NewLine + "stickyHeaders_yScroll : null,";
                l_Overview += Environment.NewLine + "// scroll table top into view after filtering";
                l_Overview += Environment.NewLine + "stickyHeaders_filteredToTop: true}});});";
                l_Overview += Environment.NewLine + "</script>";
                //
                l_Overview += Environment.NewLine + "<body>";
                //l_Overview += Environment.NewLine + "<table id=\"table1\" summary=\"Grepolis Town Overview\">";
                l_Overview += Environment.NewLine + "<table id=\"grepolis\" class=\"tablesorter\" summary=\"Grepolis Town Overview\">";
                l_Overview += Environment.NewLine + "<thead>";
                l_Overview += Environment.NewLine + "<tr>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"trademode_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"wood_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"stone_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"iron_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"free_pop_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"storage_25x25.png\" /></th>";
                //l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"points_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"main_20x20.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"barracks_20x20.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"docks_20x20.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"farmer_20x20.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"quest_island_25x25.png\" /></th>";
                //l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"free_trade_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"attack_in.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"favor_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"sword_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"slinger_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"archer_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"hoplite_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"rider_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"chariot_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"catapult_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"minotaur_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"manticore_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"centaur_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"pegasus_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"harpy_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"medusa_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"zyklop_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"cerberus_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"fury_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"griffin_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"calydonian_boar_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"godsent_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"big_transporter_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"bireme_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"attack_ship_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"demolition_ship_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"small_transporter_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"trireme_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"colonize_ship_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"sea_monster_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "</tr>";
                l_Overview += Environment.NewLine + "</thead>";
                l_Overview += Environment.NewLine + "<tfoot>";
                l_Overview += Environment.NewLine + "<tr>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"trademode_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"wood_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"stone_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"iron_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"free_pop_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"storage_25x25.png\" /></th>";
                //l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"points_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"main_20x20.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"barracks_20x20.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"docks_20x20.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"farmer_20x20.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"quest_island_25x25.png\" /></th>";
                //l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"free_trade_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"attack_in.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"favor_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"sword_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"slinger_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"archer_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"hoplite_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"rider_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"chariot_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"catapult_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"minotaur_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"manticore_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"centaur_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"pegasus_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"harpy_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"medusa_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"zyklop_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"cerberus_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"fury_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"griffin_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"calydonian_boar_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"godsent_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"big_transporter_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"bireme_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"attack_ship_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"demolition_ship_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"small_transporter_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"trireme_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"colonize_ship_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "<th scope=\"col\"><img src=\"sea_monster_25x25.png\" /></th>";
                l_Overview += Environment.NewLine + "</tr>";
                l_Overview += Environment.NewLine + "<tr><td colspan=\"100%\"><em>Grepolis Town Overview - " + DateTime.Now + "</em></td></tr>";
                l_Overview += Environment.NewLine + "</tfoot>";
                l_Overview += Environment.NewLine + "<tbody>";

                for (int i = 0; i < p_Player.Towns.Count; i++)
                {
                    l_Overview += Environment.NewLine + "<tr>";
                    l_Overview += Environment.NewLine + "<td nowrap scope=\"col\">" + p_Player.Towns[i].Name + "</td>";

                    l_God = "";
                    if (p_Player.Towns[i].God.Equals("zeus"))
                        l_God = "<img title=\"" + p_Player.getFavorByTownIndex(i) + "\" src=\"zeus_mini.png\" />";
                    else if (p_Player.Towns[i].God.Equals("poseidon"))
                        l_God = "<img title=\"" + p_Player.getFavorByTownIndex(i) + "\" src=\"poseidon_mini.png\" />";
                    else if (p_Player.Towns[i].God.Equals("hera"))
                        l_God = "<img title=\"" + p_Player.getFavorByTownIndex(i) + "\" src=\"hera_mini.png\" />";
                    else if (p_Player.Towns[i].God.Equals("athena"))
                        l_God = "<img title=\"" + p_Player.getFavorByTownIndex(i) + "\" src=\"athena_mini.png\" />";
                    else if (p_Player.Towns[i].God.Equals("hades"))
                        l_God = "<img title=\"" + p_Player.getFavorByTownIndex(i) + "\" src=\"hades_mini.png\" />";
                    else if (p_Player.Towns[i].God.Equals("artemis"))
                        l_God = "<img title=\"" + p_Player.getFavorByTownIndex(i) + "\" src=\"artemis_mini.png\" />";
                    else
                        l_God = "";

                    if (p_Player.Towns[i].TradeEnabled)
                    {
                        if (p_Player.Towns[i].TradeMode.Equals("send"))
                            l_TradeMode = "S";
                        else if (p_Player.Towns[i].TradeMode.Equals("receive"))
                            l_TradeMode = "R";
                        else
                            l_TradeMode = "Spy";
                    }
                    else
                    {
                        l_TradeMode = "-";
                    }

                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + l_God + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + l_TradeMode + "</td>";
                    if (p_Player.Towns[i].Storage == p_Player.Towns[i].Wood)
                        l_Wood = "<td style=\"color:red;\" scope=\"col\">" + p_Player.Towns[i].Wood + "</td>";
                    else
                        l_Wood = "<td scope=\"col\">" + p_Player.Towns[i].Wood + "</td>";
                    l_Overview += Environment.NewLine + l_Wood;
                    if (p_Player.Towns[i].Storage == p_Player.Towns[i].Stone)
                        l_Stone = "<td style=\"color:red;\" scope=\"col\">" + p_Player.Towns[i].Stone + "</td>";
                    else
                        l_Stone = "<td scope=\"col\">" + p_Player.Towns[i].Stone + "</td>";
                    l_Overview += Environment.NewLine + l_Stone;
                    if (p_Player.Towns[i].Storage == p_Player.Towns[i].Iron)
                        l_Iron = "<td style=\"color:red;\" scope=\"col\">" + p_Player.Towns[i].Iron + "</td>";
                    else
                        l_Iron = "<td scope=\"col\">" + p_Player.Towns[i].Iron + "</td>";
                    l_Overview += Environment.NewLine + l_Iron;
                    l_Overview += Environment.NewLine + "<td nowrap scope=\"col\">" + p_Player.Towns[i].PopulationAvailable + "(" + p_Player.Towns[i].Buildings[3].Level + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].Storage + "</td>";
                    //l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ResearchPoints + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].getIngameBuildingQueueSize() + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].SizeOfLandUnitQueue.ToString() + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].SizeOfNavyUnitQueue.ToString() + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].getNumberOfFriendlyFarmers().ToString() + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].AvailableQuests.ToString() + "</td>";
                    //l_Overview += Environment.NewLine + "<td scope=\"col\">" + "-" + "</td>";//Trade capacity
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].getNumberOfIncomingAttacks().ToString() + "</td>";

                    l_ActiveBuffs = "";
                    if (p_Player.Towns[i].isPowerActive("call_of_the_ocean"))
                        l_ActiveBuffs += "<img src=\"power_call_of_the_ocean_16x16.png\" />";
                    if (p_Player.Towns[i].isPowerActive("fertility_improvement"))
                        l_ActiveBuffs += "<img src=\"power_fertility_improvement_16x16.png\" />";
                    if (p_Player.Towns[i].isPowerActive("happiness"))
                        l_ActiveBuffs += "<img src=\"power_happiness_16x16.png\" />";
                    if (p_Player.Towns[i].isPowerActive("pest"))
                        l_ActiveBuffs += "<img src=\"power_pest_16x16.png\" />";
                    if (p_Player.Towns[i].isPowerActive("town_protection"))
                        l_ActiveBuffs += "<img src=\"power_town_protection_16x16.png\" />";

                    l_Overview += Environment.NewLine + "<td nowrap scope=\"col\">" + l_ActiveBuffs + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("sword")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("sword")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("slinger")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("slinger")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("archer")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("archer")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("hoplite")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("hoplite")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("rider")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("rider")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("chariot")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("chariot")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("catapult")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("catapult")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("minotaur")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("minotaur")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("manticore")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("manticore")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("centaur")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("centaur")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("pegasus")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("pegasus")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("harpy")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("harpy")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("medusa")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("medusa")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("zyklop")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("zyklop")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("cerberus")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("cerberus")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("fury")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("fury")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("griffin")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("griffin")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("calydonian_boar")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("calydonian_boar")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("godsent")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("godsent")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("big_transporter")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("big_transporter")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("bireme")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("bireme")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("attack_ship")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("attack_ship")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("demolition_ship")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("demolition_ship")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("small_transporter")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("small_transporter")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("trireme")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("trireme")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("colonize_ship")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("colonize_ship")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("sea_monster")].CurrentAmount + " (" + p_Player.Towns[i].ArmyUnits[p_Player.Towns[i].getUnitIndex("sea_monster")].TotalAmount + ")" + "</td>";
                    l_Overview += Environment.NewLine + "</tr>";
                }

                //Total
                l_Overview += Environment.NewLine + "<tr>";
                l_Overview += Environment.NewLine + "<td nowrap scope=\"col\">" + "Total" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + "</td>";
                l_Overview += Environment.NewLine + "<td nowrap scope=\"col\">" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + "</td>";
                //l_Overview += Environment.NewLine + "<td scope=\"col\">" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + "</td>";
                //l_Overview += Environment.NewLine + "<td scope=\"col\">" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + "</td>";
                l_Overview += Environment.NewLine + "<td nowrap scope=\"col\">" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("sword") + " (" + p_Player.getTotalUnitsAll("sword") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("slinger") + " (" + p_Player.getTotalUnitsAll("slinger") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("archer") + " (" + p_Player.getTotalUnitsAll("archer") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("hoplite") + " (" + p_Player.getTotalUnitsAll("hoplite") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("rider") + " (" + p_Player.getTotalUnitsAll("rider") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("chariot") + " (" + p_Player.getTotalUnitsAll("chariot") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("catapult") + " (" + p_Player.getTotalUnitsAll("catapult") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("minotaur") + " (" + p_Player.getTotalUnitsAll("minotaur") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("manticore") + " (" + p_Player.getTotalUnitsAll("manticore") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("centaur") + " (" + p_Player.getTotalUnitsAll("centaur") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("pegasus") + " (" + p_Player.getTotalUnitsAll("pegasus") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("harpy") + " (" + p_Player.getTotalUnitsAll("harpy") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("medusa") + " (" + p_Player.getTotalUnitsAll("medusa") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("zyklop") + " (" + p_Player.getTotalUnitsAll("zyklop") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("cerberus") + " (" + p_Player.getTotalUnitsAll("cerberus") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("fury") + " (" + p_Player.getTotalUnitsAll("fury") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("griffin") + " (" + p_Player.getTotalUnitsAll("griffin") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("calydonian_boar") + " (" + p_Player.getTotalUnitsAll("calydonian_boar") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("godsent") + " (" + p_Player.getTotalUnitsAll("godsent") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("big_transporter") + " (" + p_Player.getTotalUnitsAll("big_transporter") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("bireme") + " (" + p_Player.getTotalUnitsAll("bireme") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("attack_ship") + " (" + p_Player.getTotalUnitsAll("attack_ship") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("demolition_ship") + " (" + p_Player.getTotalUnitsAll("demolition_ship") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("small_transporter") + " (" + p_Player.getTotalUnitsAll("small_transporter") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("trireme") + " (" + p_Player.getTotalUnitsAll("trireme") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("colonize_ship") + " (" + p_Player.getTotalUnitsAll("colonize_ship") + ")" + "</td>";
                l_Overview += Environment.NewLine + "<td scope=\"col\">" + p_Player.getTotalUnits("sea_monster") + " (" + p_Player.getTotalUnitsAll("sea_monster") + ")" + "</td>";
                l_Overview += Environment.NewLine + "</tr>";

                l_Overview += Environment.NewLine + "</tbody>";
                l_Overview += Environment.NewLine + "</table>";
                l_Overview += Environment.NewLine + "</body>";
                l_Overview += Environment.NewLine + "</html>";
            }
            catch (Exception)
            {

            }

            return l_Overview;
        }
    }
}
