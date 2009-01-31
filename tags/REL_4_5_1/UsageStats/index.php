<?php
/*
(C) 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

// If adding a new table to the database don't forget to update MySQL::record_count!

/* Changelog:
1.0: First production version in support of AWB 4.3
1.1: Database collation fixed
1.2: New and improved output queries; fix for Wikimedia Foundation monolingual projects (meta, commons, species)
1.3: Bug fix for Simple English Wikipedia; add script version to output stats
1.4: Whoops, try again for Simple English (and I'll test it this time!)
1.5: Layout standardisation. URL check for WowWiki. Make table cells sortable. Extend use of CSS.
1.6: Add 127.0.0.1 to table as 'localhost'. Display sites without a dot in the domain name string as '<intranet>'.
*/
define(MAJOR, 1); define(MINOR, 6); // TINYINTS; 10 is higher than (0)9

/* Variables:
Action: Hello, Update
Version: AWB version
Wiki
Language
Culture
User
OS
Debug
RecordID: when updating
Verify: random number generated by this script and returned by AWB when sending an update
Saves: number of saved edits
PluginCount: number of installed plugins (hello) or new plugins (update)
P1N, P2N etc: name of plugin
P1V, P2V etc: plugin version
*/

/* I would have preferred to write a web service, but I don't have access to an ASP.NET server and
Dreamhost's default PHP binary doesn't support SOAP. Since we're not doing anything particularly
complex it seemed easiest to use POST and create/return some simple XML ourselves. */

// TODO: A bot or a DEBUG mode button in AWB to write some summary stats to a WP page

/* @var $db DB */ // Hint for Zend Studio autocomplete, don't delete
require_once("config.php");
require_once("includes/MySQL.php");

// we don't want proxy servers caching anything:
header('Cache-Control: no-cache, no-store, must-revalidate'); //HTTP/1.1
header('Expires: Sun, 01 Jul 2005 00:00:00 GMT');
header('Pragma: no-cache'); //HTTP/1.0

ob_start(); // buffering allows us to create headers deep into the script (in theory! didn't seem to work in practice) and also to get rid of warning/error messages
global $db;
$db=new DB();
$db->db_connect();

switch ($_POST["Action"]) {
case "Hello":
	header('Content-Type: text/xml');
	FirstContact();
	FinishUp($xmlout);
	break;
	
case "Update":
	header('Content-Type: text/html; charset=utf-8');
	$db->update_usage_record();
	FinishUp("OK");
	break;
	
case "Stats":
	// return some stats in wiki code? (plain txt)
	break;
	
default:
	header('Content-Type: text/html; charset=utf-8');
	//$db->init_log(3); // uncomment if something is broken and need to log error messages (note that will always be logged as failing unless the Log_Success method is modified and called)
	require_once("includes/Stats.php");
	htmlstats();
	FinishUp(ob_get_contents());
}

function FirstContact() {	
	global $db, $xmlout;		
	$db->init_log(1);
	
	$time=localtime(time(), true);
	$VerifyID=$time['tm_min']+$time['tm_sec'];
	$RecordID=$db->add_usage_record($VerifyID);
	
	$memory = xmlwriter_open_memory();
	xmlwriter_start_document($memory, '1.0', 'UTF-8');
	xmlwriter_write_dtd($memory, "AWB");
	xmlwriter_start_element ($memory, "DB");
	xmlwriter_write_attribute($memory, "Record", $RecordID);
	xmlwriter_write_attribute($memory, "Verify", $VerifyID);
	xmlwriter_end_element($memory);	
	$xmlout=xmlwriter_output_memory($memory, true);
}

function FinishUp($output) {	
	global $db;
	$db->db_disconnect();
	
	ob_end_clean(); // gets rid of warning messages etc; comment out if want to see those
	print $output;
}

// Call this rather than die() directly, so that AWB can always parse for "Error: "
function dead($msg) {	
	global $db;
	$db->log_error($msg);
	$db->db_disconnect();
	
	header("Barf", true, 500); // comment out for in-browser debugging
	ob_end_clean(); // gets rid of warning messages etc; comment out if want to see those
	die("Error: $msg");
}
?>