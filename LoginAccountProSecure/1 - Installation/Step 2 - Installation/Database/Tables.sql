-- phpMyAdmin SQL Dump
-- version 4.0.10.7
-- http://www.phpmyadmin.net
-- Version PHP: 5.4.31

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- `Account`
--

CREATE TABLE IF NOT EXISTS `Account` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `mail` varchar(255) NOT NULL,
  `username` varchar(255) NOT NULL,
  `password` text NOT NULL,
  `salt` varchar(255) NOT NULL,
  `admin` int(1) NOT NULL DEFAULT '0',
  `validation_code` varchar(255) NOT NULL,
  `validated` int(11) NOT NULL,
  `banned` int(1) NOT NULL DEFAULT '0',
  `creation_date` datetime NOT NULL,
  `session_token` varchar(255) DEFAULT NULL,
  `last_activity` datetime NOT NULL,
  `last_connection_date` datetime NOT NULL,
  `Data1` varchar(255) DEFAULT NULL,
  `Data2` varchar(255) DEFAULT NULL,
  `Data3` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mail` (`mail`,`username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- `Attempts`
--

CREATE TABLE IF NOT EXISTS `Attempts` (
  `account_id` int(11) NOT NULL,
  `ip` varchar(255) CHARACTER SET utf8 NOT NULL,
  `action` varchar(255) CHARACTER SET utf8 NOT NULL,
  `attempts` int(11) NOT NULL,
  PRIMARY KEY (`account_id`,`ip`,`action`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- `IP`
--

CREATE TABLE IF NOT EXISTS `IP` (
  `account_id` bigint(20) NOT NULL,
  `ip` varchar(255) NOT NULL,
  `validation_code` varchar(255) NOT NULL,
  `validated` int(11) NOT NULL,
  `creation_date` date NOT NULL,
  PRIMARY KEY (`account_id`,`ip`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- `Report`
--

CREATE TABLE IF NOT EXISTS `Report` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `creation_date` date NOT NULL,
  `reporter_id` bigint(20) NOT NULL,
  `done_date` date DEFAULT NULL,
  `message` text CHARACTER SET utf8,
  `screenshot` longtext CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- `SaveGame`
--

CREATE TABLE IF NOT EXISTS `SaveGame` (
  `account_id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `file` longtext NOT NULL,
  PRIMARY KEY (`account_id`,`name`),
  UNIQUE KEY `account_id` (`account_id`,`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
