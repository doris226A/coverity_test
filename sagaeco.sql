-- phpMyAdmin SQL Dump
-- version 4.9.0.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Generation Time: Jul 13, 2019 at 03:43 PM
-- Server version: 5.6.5-m8
-- PHP Version: 7.3.6

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `sagaeco`
--
CREATE DATABASE IF NOT EXISTS `sagaeco5` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;
USE `sagaeco5`;

-- --------------------------------------------------------

--
-- Table structure for table `another_paper`
--

DROP TABLE IF EXISTS `another_paper`;
CREATE TABLE IF NOT EXISTS `another_paper` (
  `paper_id` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `char_id` int(10) DEFAULT NULL,
  `paper_value` bigint(20) UNSIGNED DEFAULT NULL,
  `paper_lv` tinyint(3) UNSIGNED DEFAULT NULL,
  PRIMARY KEY (`paper_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `another_paper`
--

TRUNCATE TABLE `another_paper`;
-- --------------------------------------------------------

--
-- Table structure for table `apiitem`
--

DROP TABLE IF EXISTS `apiitem`;
CREATE TABLE IF NOT EXISTS `apiitem` (
  `apiitem_id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `char_id` int(10) UNSIGNED NOT NULL,
  `item_id` int(10) UNSIGNED NOT NULL,
  `qty` smallint(5) UNSIGNED NOT NULL DEFAULT '1',
  `request_time` datetime NOT NULL,
  `process_time` datetime DEFAULT NULL,
  `status` tinyint(4) NOT NULL DEFAULT '0',
  PRIMARY KEY (`apiitem_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `apiitem`
--

TRUNCATE TABLE `apiitem`;
-- --------------------------------------------------------

--
-- Table structure for table `avar`
--

DROP TABLE IF EXISTS `avar`;
CREATE TABLE IF NOT EXISTS `avar` (
  `account_id` int(10) UNSIGNED NOT NULL,
  `values` blob NOT NULL,
  PRIMARY KEY (`account_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `avar`
--

TRUNCATE TABLE `avar`;
-- --------------------------------------------------------

--
-- Table structure for table `bbs`
--

DROP TABLE IF EXISTS `bbs`;
CREATE TABLE IF NOT EXISTS `bbs` (
  `postid` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `bbsid` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `postdate` datetime NOT NULL DEFAULT '1970-01-01 00:00:00',
  `charid` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `name` varchar(30) NOT NULL DEFAULT ' ',
  `title` varchar(256) NOT NULL DEFAULT ' ',
  `content` varchar(256) NOT NULL DEFAULT ' ',
  PRIMARY KEY (`postid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `bbs`
--

TRUNCATE TABLE `bbs`;
-- --------------------------------------------------------

--
-- Table structure for table `char`
--

DROP TABLE IF EXISTS `char`;
CREATE TABLE IF NOT EXISTS `char` (
  `char_id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `account_id` int(10) UNSIGNED NOT NULL,
  `name` varchar(30) NOT NULL,
  `firstname` varchar(30) NOT NULL DEFAULT 'Testing',
  `showfirstname` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `race` tinyint(4) UNSIGNED NOT NULL,
  `anotherpaperbookunlock` tinyint(3) unsigned NOT NULL default '0',
  `usingpaper_id` smallint(5) UNSIGNED NOT NULL DEFAULT '0',
  `title_id` smallint(5) UNSIGNED NOT NULL DEFAULT '0',
  `gender` tinyint(3) UNSIGNED NOT NULL,
  `hairStyle` smallint(5) UNSIGNED NOT NULL,
  `hairColor` tinyint(3) UNSIGNED NOT NULL,
  `wig` smallint(5) UNSIGNED NOT NULL,
  `face` smallint(5) UNSIGNED NOT NULL,
  `baseHairStyle` smallint(5) unsigned NOT NULL,
  `baseHairColor` tinyint(3) unsigned NOT NULL,
  `baseWig` smallint(5) unsigned NOT NULL,
  `baseFace` smallint(5) unsigned NOT NULL,
  `job` tinyint(3) UNSIGNED NOT NULL,
  `jobbase` int(10) unsigned NOT NULL default '0',
  `lv` tinyint(3) UNSIGNED NOT NULL,
  `lv1` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `jointjlv` tinyint(3) UNSIGNED NOT NULL DEFAULT '1',
  `jlv1` tinyint(3) UNSIGNED NOT NULL,
  `jlv2x` tinyint(3) UNSIGNED NOT NULL,
  `jlv2t` tinyint(3) UNSIGNED NOT NULL,
  `jlv3` tinyint(3) UNSIGNED NOT NULL,
  `questRemaining` smallint(5) UNSIGNED NOT NULL,
  `fame` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `questresettime` datetime NOT NULL DEFAULT '2000-01-01 00:00:00',
  `slot` tinyint(3) UNSIGNED DEFAULT NULL,
  `mapID` int(10) UNSIGNED NOT NULL,
  `x` tinyint(3) UNSIGNED NOT NULL,
  `y` tinyint(3) UNSIGNED NOT NULL,
  `save_map` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `save_x` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `save_y` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `dir` tinyint(3) UNSIGNED NOT NULL,
  `hp` int(10) UNSIGNED NOT NULL,
  `max_hp` int(10) UNSIGNED NOT NULL,
  `mp` int(10) UNSIGNED NOT NULL,
  `max_mp` int(10) UNSIGNED NOT NULL,
  `sp` int(10) UNSIGNED NOT NULL,
  `max_sp` int(10) UNSIGNED NOT NULL,
  `ep` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `eplogindate` datetime NOT NULL DEFAULT '2000-01-01 00:00:00',
  `epgreetingdate` datetime NOT NULL DEFAULT '2000-01-01 00:00:00',
  `epused` smallint(6) NOT NULL DEFAULT '0',
  `tailStyle` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `wingStyle` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `wingColor` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `online` tinyint(4) NOT NULL DEFAULT '0',
  `cl` smallint(6) NOT NULL DEFAULT '9',
  `str` smallint(5) UNSIGNED NOT NULL,
  `dex` smallint(5) UNSIGNED NOT NULL,
  `intel` smallint(5) UNSIGNED NOT NULL,
  `vit` smallint(5) UNSIGNED NOT NULL,
  `agi` smallint(6) UNSIGNED NOT NULL,
  `mag` smallint(6) UNSIGNED NOT NULL,
  `statspoint` smallint(5) UNSIGNED NOT NULL DEFAULT '0',
  `skillpoint` smallint(5) UNSIGNED NOT NULL DEFAULT '0',
  `skillpoint2x` smallint(5) UNSIGNED NOT NULL DEFAULT '0',
  `skillpoint2t` smallint(5) UNSIGNED NOT NULL DEFAULT '0',
  `skillpoint3` smallint(5) UNSIGNED NOT NULL DEFAULT '0',
  `explorerEXP` bigint(10) UNSIGNED NOT NULL DEFAULT '0',
  `gold` bigint(10) NOT NULL DEFAULT '0',
  `cp` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `ecoin` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `cexp` bigint(10) UNSIGNED NOT NULL DEFAULT '0',
  `jexp` bigint(10) UNSIGNED NOT NULL DEFAULT '0',
  `jjexp` bigint(10) UNSIGNED NOT NULL DEFAULT '0',
   `pageexp` bigint(20) unsigned NOT NULL default '0',
  `wrp` int(11) NOT NULL DEFAULT '0',
  `possession_target` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `questid` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `questendtime` datetime DEFAULT NULL,
  `queststatus` tinyint(3) UNSIGNED NOT NULL DEFAULT '1',
  `questcurrentcount1` int(11) NOT NULL DEFAULT '0',
  `questcurrentcount2` int(11) NOT NULL DEFAULT '0',
  `questcurrentcount3` int(11) NOT NULL DEFAULT '0',
  `party` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `ring` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `golem` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `WaitType` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `abyssfloor` int(10) NOT NULL default '0',
  `DualJobID` tinyint(3) unsigned NOT NULL default '0',
  `ExStatPoint` smallint(5) unsigned NOT NULL default '0',
  `ExSkillPoint` tinyint(3) unsigned NOT NULL default '0',
  `LastPaperRelease` datetime NOT NULL default '2000-01-01 00:00:00',
  PRIMARY KEY (`char_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `char`
--

TRUNCATE TABLE `char`;
-- --------------------------------------------------------

--
-- Table structure for table `cvar`
--

DROP TABLE IF EXISTS `cvar`;
CREATE TABLE IF NOT EXISTS `cvar` (
  `char_id` int(10) UNSIGNED NOT NULL,
  `values` blob NOT NULL,
  PRIMARY KEY (`char_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `cvar`
--

TRUNCATE TABLE `cvar`;
-- --------------------------------------------------------

--
-- Table structure for table `dualjob`
--

DROP TABLE IF EXISTS `dualjob`;
CREATE TABLE IF NOT EXISTS `dualjob` (
  `recordID` varchar(36) NOT NULL,
  `char_id` int(10) UNSIGNED NOT NULL,
  `series_id` int(10) UNSIGNED NOT NULL,
  `level` tinyint(3) UNSIGNED NOT NULL DEFAULT '1',
  `exp` bigint(20) UNSIGNED DEFAULT '0',
  PRIMARY KEY (`recordID`),
  KEY `searchIndex` (`char_id`,`series_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='副职业相关';

--
-- Truncate table before insert `dualjob`
--

TRUNCATE TABLE `dualjob`;
--
-- Triggers `dualjob`
--
DROP TRIGGER IF EXISTS `tgDualJobID`;
DELIMITER $$
CREATE TRIGGER `tgDualJobID` BEFORE INSERT ON `dualjob` FOR EACH ROW BEGIN
	set NEW.recordid = uuid();
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `dualjob_skill`
--

DROP TABLE IF EXISTS `dualjob_skill`;
CREATE TABLE IF NOT EXISTS `dualjob_skill` (
  `recordid` varchar(36) NOT NULL,
  `char_id` int(10) UNSIGNED NOT NULL,
  `series_id` tinyint(3) UNSIGNED NOT NULL,
  `skill_id` int(10) UNSIGNED NOT NULL,
  `skill_level` tinyint(3) UNSIGNED NOT NULL,
  PRIMARY KEY (`recordid`),
  KEY `char_id_series_id` (`char_id`,`series_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='副职技能记录';

--
-- Truncate table before insert `dualjob_skill`
--

TRUNCATE TABLE `dualjob_skill`;
--
-- Triggers `dualjob_skill`
--
DROP TRIGGER IF EXISTS `tgDualJobSkillID`;
DELIMITER $$
CREATE TRIGGER `tgDualJobSkillID` BEFORE INSERT ON `dualjob_skill` FOR EACH ROW BEGIN
	SET NEW.recordid = uuid();
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `ff`
--

DROP TABLE IF EXISTS `ff`;
CREATE TABLE IF NOT EXISTS `ff` (
  `ff_id` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `ring_id` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `name` varchar(50) NOT NULL DEFAULT '',
  `content` text NOT NULL,
  `level` int(10) UNSIGNED NOT NULL DEFAULT '0',
  PRIMARY KEY (`ff_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `ff`
--

TRUNCATE TABLE `ff`;
-- --------------------------------------------------------

--
-- Table structure for table `ff_furniture`
--

DROP TABLE IF EXISTS `ff_furniture`;
CREATE TABLE IF NOT EXISTS `ff_furniture` (
  `ff_id` int(10) NOT NULL DEFAULT '0',
  `place` tinyint(3) NOT NULL DEFAULT '0',
  `item_id` int(10) NOT NULL DEFAULT '0',
  `pict_id` int(10) NOT NULL DEFAULT '0',
  `x` smallint(6) NOT NULL DEFAULT '0',
  `y` smallint(6) NOT NULL DEFAULT '0',
  `z` smallint(6) NOT NULL DEFAULT '0',
  `xaxis` smallint(6) NOT NULL DEFAULT '0',
  `yaxis` smallint(6) NOT NULL DEFAULT '0',
  `zaxis` smallint(6) NOT NULL DEFAULT '0',
  `motion` smallint(11) NOT NULL DEFAULT '0',
  `name` varchar(50) NOT NULL DEFAULT ''
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `ff_furniture`
--

TRUNCATE TABLE `ff_furniture`;
-- --------------------------------------------------------

--
-- Table structure for table `fgarden`
--

DROP TABLE IF EXISTS `fgarden`;
CREATE TABLE IF NOT EXISTS `fgarden` (
  `fgarden_id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `account_id` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `part1` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `part2` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `part3` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `part4` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `part5` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `part6` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `part7` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `part8` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `fuel` int(10) UNSIGNED NOT NULL DEFAULT '0',
  PRIMARY KEY (`fgarden_id`),
  KEY `account_id` (`account_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `fgarden`
--

TRUNCATE TABLE `fgarden`;
-- --------------------------------------------------------

--
-- Table structure for table `fgarden_furniture`
--

DROP TABLE IF EXISTS `fgarden_furniture`;
CREATE TABLE IF NOT EXISTS `fgarden_furniture` (
  `fgarden_id` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `place` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `item_id` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `pict_id` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `x` smallint(6) NOT NULL DEFAULT '0',
  `y` smallint(6) NOT NULL DEFAULT '0',
  `z` smallint(6) NOT NULL DEFAULT '0',
  `xaxis` smallint(6) NOT NULL DEFAULT '0',
  `yaxis` smallint(6) NOT NULL DEFAULT '0',
  `zaxis` smallint(6) NOT NULL DEFAULT '0',
  `dir` smallint(5) UNSIGNED NOT NULL DEFAULT '0',
  `motion` smallint(5) UNSIGNED NOT NULL DEFAULT '0',
  `name` varchar(50) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL DEFAULT ' ',
  KEY `fgarden_id` (`fgarden_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `fgarden_furniture`
--

TRUNCATE TABLE `fgarden_furniture`;
--
-- Dumping data for table `fgarden_furniture`
--

INSERT DELAYED INTO `fgarden_furniture` (`fgarden_id`, `place`, `item_id`, `pict_id`, `x`, `y`, `z`, `xaxis`, `yaxis`, `zaxis`, `dir`, `motion`, `name`) VALUES
(4, 0, 31118700, 0, 69, 0, 110, 0, 0, 0, 0, 111, 'ヴァルハラ'),
(5, 0, 31060700, 0, 234, 0, -109, 0, 0, 0, 0, 111, 'キューブ１×１'),
(5, 0, 31060700, 0, 105, 0, 57, 0, 0, 0, 0, 111, 'キューブ１×１'),
(5, 0, 31060700, 0, -313, 50, -127, 0, 0, 0, 0, 111, 'キューブ１×１'),
(5, 0, 31060700, 0, -114, 0, 121, 0, 0, 0, 0, 111, 'キューブ１×１'),
(5, 0, 31060700, 0, -36, 0, -143, 0, 0, 0, 0, 111, 'キューブ１×１');

-- --------------------------------------------------------

--
-- Table structure for table `friend`
--

DROP TABLE IF EXISTS `friend`;
CREATE TABLE IF NOT EXISTS `friend` (
  `char_id` int(10) UNSIGNED NOT NULL,
  `friend_char_id` int(10) UNSIGNED NOT NULL,
  KEY `char_id` (`char_id`),
  KEY `friend_char_id` (`friend_char_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `friend`
--

TRUNCATE TABLE `friend`;
-- --------------------------------------------------------

--
-- Table structure for table `gifts`
--

DROP TABLE IF EXISTS `gifts`;
CREATE TABLE IF NOT EXISTS `gifts` (
  `GiftID` int(10) NOT NULL AUTO_INCREMENT,
  `a_id` int(11) NOT NULL DEFAULT '0',
  `mail_id` int(11) NOT NULL DEFAULT '0',
  `sender` varchar(50) NOT NULL,
  `title` varchar(50) NOT NULL,
  `postdate` datetime NOT NULL,
  `itemid1` int(11) NOT NULL,
  `itemid2` int(11) NOT NULL,
  `itemid3` int(11) NOT NULL,
  `itemid4` int(11) NOT NULL,
  `itemid5` int(11) NOT NULL,
  `itemid6` int(11) NOT NULL,
  `itemid7` int(11) NOT NULL,
  `itemid8` int(11) NOT NULL,
  `itemid9` int(11) NOT NULL,
  `itemid10` int(11) NOT NULL,
  `count1` int(11) NOT NULL,
  `count2` int(11) NOT NULL,
  `count3` int(11) NOT NULL,
  `count4` int(11) NOT NULL,
  `count5` int(11) NOT NULL,
  `count6` int(11) NOT NULL,
  `count7` int(11) NOT NULL,
  `count8` int(11) NOT NULL,
  `count9` int(11) NOT NULL,
  `count10` int(11) NOT NULL,
  PRIMARY KEY (`GiftID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `gifts`
--

TRUNCATE TABLE `gifts`;
-- --------------------------------------------------------

--
-- Table structure for table `inventory`
--

DROP TABLE IF EXISTS `inventory`;
CREATE TABLE IF NOT EXISTS `inventory` (
  `char_id` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `data` blob,
  PRIMARY KEY (`char_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `inventory`
--

TRUNCATE TABLE `inventory`;
-- --------------------------------------------------------

--
-- Table structure for table `levellimit`
--

DROP TABLE IF EXISTS `levellimit`;
CREATE TABLE IF NOT EXISTS `levellimit` (
  `NowLevelLimit` int(10) NOT NULL DEFAULT '30',
  `NextLevelLimit` int(10) NOT NULL DEFAULT '40',
  `SetNextUpLevel` int(10) NOT NULL,
  `LastTimeLevelLimit` int(10) NOT NULL DEFAULT '110',
  `SetNextUpDays` int(10) NOT NULL DEFAULT '30',
  `ReachTime` datetime NOT NULL,
  `NextTime` datetime NOT NULL,
  `FirstPlayer` int(10) NOT NULL DEFAULT '0',
  `SecondPlayer` int(10) NOT NULL DEFAULT '0',
  `ThirdPlayer` int(10) NOT NULL DEFAULT '0',
  `FourthPlayer` int(10) NOT NULL DEFAULT '0',
  `FifthPlayer` int(10) NOT NULL DEFAULT '0',
  `IsLock` tinyint(3) NOT NULL DEFAULT '0'
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `levellimit`
--

TRUNCATE TABLE `levellimit`;
-- --------------------------------------------------------

--
-- Table structure for table `log`
--

DROP TABLE IF EXISTS `log`;
CREATE TABLE IF NOT EXISTS `log` (
  `eventType` varchar(20) NOT NULL,
  `eventTime` datetime NOT NULL,
  `src` varchar(50) NOT NULL,
  `dst` varchar(50) DEFAULT NULL,
  `detail` varchar(1024) DEFAULT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `log`
--

TRUNCATE TABLE `log`;
-- --------------------------------------------------------

--
-- Table structure for table `login`
--

DROP TABLE IF EXISTS `login`;
CREATE TABLE IF NOT EXISTS `login` (
  `account_id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `username` varchar(30) NOT NULL,
  `password` varchar(32) NOT NULL,
  `deletepass` varchar(32) NOT NULL,
  `banned` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `gmlevel` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `bank` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `vshop_points` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `used_vshop_points` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `lastip` varchar(20) DEFAULT NULL,
  `questresettime` datetime DEFAULT '2000-01-01 00:00:00',
  `lastlogintime` datetime DEFAULT '2000-01-01 00:00:00',
  `macaddress` varchar(15) DEFAULT 'now()',
  `playernames` varchar(50) DEFAULT 'now()',
  PRIMARY KEY (`account_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `login`
--

TRUNCATE TABLE `login`;
-- --------------------------------------------------------

--
-- Table structure for table `mails`
--

DROP TABLE IF EXISTS `mails`;
CREATE TABLE IF NOT EXISTS `mails` (
  `char_id` int(10) UNSIGNED DEFAULT NULL,
  `name` mediumtext,
  `title` mediumtext,
  `content` mediumtext,
  `postdate` datetime DEFAULT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='邮件用sql\r\n';

--
-- Truncate table before insert `mails`
--

TRUNCATE TABLE `mails`;
-- --------------------------------------------------------

--
-- Table structure for table `mirror`
--

DROP TABLE IF EXISTS `mirror`;
CREATE TABLE `mirror` (
  `char_id` int(10) UNSIGNED NOT NULL,
  `default_value` smallint(5) NULL NULL,
  `face_info` smallint(5) NULL NULL,
  `hair_info` smallint(5) NULL NULL,
  `wig_info` smallint(5) NULL NULL,
  `hair_color_info` tinyint(3) NOT NULL DEFAULT 0
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
TRUNCATE TABLE `mirror`;
-- --------------------------------------------------------

--
-- Table structure for table `mobstates`
--

DROP TABLE IF EXISTS `mobstates`;
CREATE TABLE IF NOT EXISTS `mobstates` (
  `char_id` int(10) UNSIGNED NOT NULL,
  `mob_id` int(10) UNSIGNED NOT NULL,
  `state` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`char_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `mobstates`
--

TRUNCATE TABLE `mobstates`;
-- --------------------------------------------------------

--
-- Table structure for table `npcstates`
--

DROP TABLE IF EXISTS `npcstates`;
CREATE TABLE IF NOT EXISTS `npcstates` (
  `char_id` int(10) UNSIGNED NOT NULL,
  `npc_id` int(10) UNSIGNED NOT NULL,
  `state` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`char_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `npcstates`
--

TRUNCATE TABLE `npcstates`;
-- --------------------------------------------------------

--
-- Table structure for table `partner`
--

DROP TABLE IF EXISTS `partner`;
CREATE TABLE `partner` (
  `apid` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `pid` int(10) unsigned NOT NULL,
  `name` varchar(30) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `mastername` varchar(30) CHARACTER SET utf8 COLLATE utf8_bin DEFAULT NULL,
  `lv` tinyint(3) unsigned NOT NULL,
  `tlv` tinyint(3) unsigned NOT NULL,
  `texp` bigint(10) unsigned NOT NULL DEFAULT '0',
  `rb` tinyint(3) unsigned NOT NULL,
  `rank` tinyint(3) unsigned NOT NULL,
  `perkspoints` smallint(6) unsigned NOT NULL,
  `hp` int(10) unsigned NOT NULL,
  `maxhp` int(10) unsigned NOT NULL,
  `mp` int(10) unsigned NOT NULL,
  `maxmp` int(10) unsigned NOT NULL,
  `sp` int(10) unsigned NOT NULL,
  `maxsp` int(10) unsigned NOT NULL,
  `perk0` tinyint(3) unsigned NOT NULL,
  `perk1` tinyint(3) unsigned NOT NULL,
  `perk2` tinyint(3) unsigned NOT NULL,
  `perk3` tinyint(3) unsigned NOT NULL,
  `perk4` tinyint(3) unsigned NOT NULL,
  `perk5` tinyint(3) unsigned NOT NULL,
  `aimode` tinyint(3) unsigned NOT NULL,
  `basicai1` tinyint(3) unsigned NOT NULL,
  `basicai2` tinyint(3) unsigned NOT NULL,
  `exp` bigint(10) unsigned NOT NULL DEFAULT '0',
  `pictid` int(10) unsigned NOT NULL DEFAULT '0',
  `nextfeedtime` datetime DEFAULT '2000-01-01 00:00:00',
  `reliabilityuprate` smallint(6) unsigned DEFAULT '0',
  PRIMARY KEY (`apid`)
) ENGINE=InnoDB AUTO_INCREMENT=5252 DEFAULT CHARSET=utf8;

-- --------------------------------------------------------
TRUNCATE TABLE `partner`;
-- ----------------------------
-- Table structure for partnerai
-- ----------------------------
DROP TABLE IF EXISTS `partnerai`;
CREATE TABLE `partnerai` (
  `apid` int(10) unsigned NOT NULL,
  `type` tinyint(3) unsigned DEFAULT NULL,
  `index` tinyint(3) unsigned DEFAULT NULL,
  `value` int(10) unsigned DEFAULT NULL,
  PRIMARY KEY (`apid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
TRUNCATE TABLE `partnerai`;
-- ----------------------------
-- Table structure for partnercube
-- ----------------------------
DROP TABLE IF EXISTS `partnercube`;
CREATE TABLE `partnercube` (
  `apid` int(10) unsigned NOT NULL,
  `type` tinyint(3) unsigned NOT NULL,
  `unique_id` smallint(10) unsigned NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
TRUNCATE TABLE `partnercube`;
-- ----------------------------
-- Table structure for partnerequip
-- ----------------------------
DROP TABLE IF EXISTS `partnerequip`;
CREATE TABLE `partnerequip` (
  `apid` int(10) unsigned NOT NULL,
  `type` tinyint(3) unsigned NOT NULL,
  `item_id` int(10) unsigned NOT NULL,
  `count` smallint(10) unsigned NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
TRUNCATE TABLE `partnerequip`;
-- ----------------------------
-- Table structure for partneritem
-- ----------------------------
DROP TABLE IF EXISTS `partneritem`;
CREATE TABLE `partneritem` (
  `apid` int(10) unsigned NOT NULL,
  `type` tinyint(3) unsigned NOT NULL,
  `item_id` int(10) unsigned NOT NULL,
  `count` smallint(6) unsigned NOT NULL,
  PRIMARY KEY (`apid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
TRUNCATE TABLE `partneritem`;
-- ----------------------------
-- Table structure for partnerskill
-- ----------------------------
DROP TABLE IF EXISTS `partnerskill`;
CREATE TABLE `partnerskill` (
  `apid` int(10) unsigned NOT NULL,
  `type` tinyint(3) unsigned NOT NULL,
  `item_id` int(10) unsigned NOT NULL,
  `count` smallint(6) unsigned NOT NULL,
  PRIMARY KEY (`apid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Table structure for table `party`
--

DROP TABLE IF EXISTS `party`;
CREATE TABLE IF NOT EXISTS `party` (
  `party_id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` varchar(30) NOT NULL,
  `leader` int(10) UNSIGNED NOT NULL DEFAULT '0',
  PRIMARY KEY (`party_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `party`
--

TRUNCATE TABLE `party`;
-- --------------------------------------------------------

--
-- Table structure for table `partymember`
--

DROP TABLE IF EXISTS `partymember`;
CREATE TABLE IF NOT EXISTS `partymember` (
  `party_id` int(10) UNSIGNED NOT NULL,
  `index` smallint(6) UNSIGNED NOT NULL AUTO_INCREMENT,
  `char_id` int(10) UNSIGNED NOT NULL,
  KEY `party_id` (`party_id`,`index`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `partymember`
--

TRUNCATE TABLE `partymember`;
-- --------------------------------------------------------

--
-- Table structure for table `questinfo`
--

DROP TABLE IF EXISTS `questinfo`;
CREATE TABLE IF NOT EXISTS `questinfo` (
  `char_id` int(10) NOT NULL,
  `object_id` int(10) NOT NULL,
  `count` int(10) NOT NULL DEFAULT '0',
  `totalcount` int(10) NOT NULL DEFAULT '0',
  `infinish` tinyint(3) NOT NULL DEFAULT '0',
  PRIMARY KEY (`char_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `questinfo`
--

TRUNCATE TABLE `questinfo`;
-- --------------------------------------------------------

--
-- Table structure for table `ring`
--

DROP TABLE IF EXISTS `ring`;
CREATE TABLE IF NOT EXISTS `ring` (
  `ring_id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL DEFAULT ' ',
  `leader` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `fame` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `emblem` blob,
  `emblem_date` datetime DEFAULT NULL,
  `ff_id` int(10) UNSIGNED DEFAULT '0',
  PRIMARY KEY (`ring_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `ring`
--

TRUNCATE TABLE `ring`;
-- --------------------------------------------------------

--
-- Table structure for table `ringmember`
--

DROP TABLE IF EXISTS `ringmember`;
CREATE TABLE IF NOT EXISTS `ringmember` (
  `ring_id` int(10) UNSIGNED NOT NULL,
  `char_id` int(10) UNSIGNED NOT NULL,
  `right` int(10) UNSIGNED NOT NULL,
  KEY `ring_id` (`ring_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `ringmember`
--

TRUNCATE TABLE `ringmember`;
-- --------------------------------------------------------

--
-- Table structure for table `skill`
--

DROP TABLE IF EXISTS `skill`;
CREATE TABLE IF NOT EXISTS `skill` (
  `char_id` int(10) UNSIGNED NOT NULL,
  `skills` longblob NOT NULL,
  `jobbasic` int(10) UNSIGNED DEFAULT NULL,
  `joblv` tinyint(3) UNSIGNED DEFAULT NULL,
  `jobexp` bigint(20) UNSIGNED DEFAULT NULL,
  `skillpoint` smallint(10) UNSIGNED DEFAULT NULL,
  `skillpoint2x` smallint(10) UNSIGNED DEFAULT NULL,
  `skillpoint2t` smallint(10) UNSIGNED DEFAULT NULL,
  `skillpoint3` smallint(10) UNSIGNED DEFAULT NULL,
  PRIMARY KEY (`char_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `skill`
--

TRUNCATE TABLE `skill`;
-- --------------------------------------------------------

--
-- Table structure for table `slist`
--

DROP TABLE IF EXISTS `slist`;
CREATE TABLE IF NOT EXISTS `slist` (
  `ServerVarID` varchar(36) DEFAULT 'uuid()',
  `name` varchar(36) DEFAULT NULL,
  `key` varchar(36) DEFAULT NULL,
  `type` tinyint(3) UNSIGNED DEFAULT NULL,
  `content` varchar(36) DEFAULT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `slist`
--

TRUNCATE TABLE `slist`;
-- --------------------------------------------------------

--
-- Table structure for table `stamp`
--

DROP TABLE IF EXISTS `stamp`;
CREATE TABLE IF NOT EXISTS `stamp` (
  `char_id` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `stamp_id` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `value` smallint(6) NOT NULL DEFAULT '0',
  PRIMARY KEY (`char_id`,`stamp_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `stamp`
--

TRUNCATE TABLE `stamp`;
-- --------------------------------------------------------

--
-- Table structure for table `svar`
--

DROP TABLE IF EXISTS `svar`;
CREATE TABLE IF NOT EXISTS `svar` (
  `name` varchar(25) NOT NULL,
  `type` tinyint(3) UNSIGNED NOT NULL DEFAULT '0',
  `content` varchar(25) NOT NULL,
  PRIMARY KEY (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `svar`
--

TRUNCATE TABLE `svar`;
-- --------------------------------------------------------

--
-- Table structure for table `tamairelending`
--

DROP TABLE IF EXISTS `tamairelending`;
CREATE TABLE IF NOT EXISTS `tamairelending` (
  `char_id` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `postdate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `comment` varchar(256) NOT NULL DEFAULT ' ',
  `renter1` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `renter2` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `renter3` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `renter4` int(10) UNSIGNED NOT NULL DEFAULT '0',
  PRIMARY KEY (`char_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `tamairelending`
--

TRUNCATE TABLE `tamairelending`;
-- --------------------------------------------------------

--
-- Table structure for table `tamairerental`
--

DROP TABLE IF EXISTS `tamairerental`;
CREATE TABLE IF NOT EXISTS `tamairerental` (
  `char_id` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `rentdate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `lender` int(10) UNSIGNED NOT NULL DEFAULT '0',
  PRIMARY KEY (`char_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `tamairerental`
--

TRUNCATE TABLE `tamairerental`;
-- --------------------------------------------------------

--
-- Table structure for table `team`
--

DROP TABLE IF EXISTS `team`;
CREATE TABLE IF NOT EXISTS `team` (
  `team_id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` varchar(30) NOT NULL,
  `leader` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member1` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member2` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member3` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member4` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member5` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member6` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member7` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member8` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member9` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member10` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member11` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member12` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member13` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member14` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member15` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member16` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member17` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member18` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member19` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member20` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member21` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member22` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member23` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member24` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member25` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member26` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member27` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member28` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member29` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `member30` int(10) UNSIGNED NOT NULL DEFAULT '0',
  PRIMARY KEY (`team_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `team`
--

TRUNCATE TABLE `team`;
-- --------------------------------------------------------

--
-- Table structure for table `titleprerequisites`
--

DROP TABLE IF EXISTS `titleprerequisites`;
CREATE TABLE IF NOT EXISTS `titleprerequisites` (
  `char_id` int(10) UNSIGNED NOT NULL,
  `prerequisite_id` int(10) UNSIGNED NOT NULL,
  `progress` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`char_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `titleprerequisites`
--

TRUNCATE TABLE `titleprerequisites`;
-- --------------------------------------------------------

--
-- Table structure for table `titlestates`
--

DROP TABLE IF EXISTS `titlestates`;
CREATE TABLE IF NOT EXISTS `titlestates` (
  `char_id` int(10) UNSIGNED NOT NULL,
  `title_id` int(10) UNSIGNED NOT NULL,
  `state` tinyint(3) NOT NULL DEFAULT '0',
  PRIMARY KEY (`char_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `titlestates`
--

TRUNCATE TABLE `titlestates`;
-- --------------------------------------------------------

--
-- Table structure for table `warehouse`
--

DROP TABLE IF EXISTS `warehouse`;
CREATE TABLE IF NOT EXISTS `warehouse` (
  `account_id` int(10) UNSIGNED NOT NULL DEFAULT '0',
  `data` blob,
  PRIMARY KEY (`account_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Truncate table before insert `warehouse`
--

TRUNCATE TABLE `warehouse`;COMMIT;

--
-- Table structure for table `timeritem`
--
DROP TABLE IF EXISTS `timeritem`;
CREATE TABLE `timeritem` (
  `timeritem_id` int(11) NOT NULL AUTO_INCREMENT,
  `char_id` int(10) NOT NULL,
  `item_id` int(11) NOT NULL,
  `start_time` int(10) UNSIGNED NOT NULL,
  `end_time` int(10) UNSIGNED NOT NULL,
  `buff_name` varchar(255) NOT NULL,
  `buffcodename` varchar(255) NOT NULL,
  `buff_values` varchar(255) NOT NULL,
  `durationtype` tinyint(1) NOT NULL,
  `duration` int(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`timeritem_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
TRUNCATE TABLE `timeritem`;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
