-- phpMyAdmin SQL Dump
-- version 5.1.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Gegenereerd op: 11 okt 2022 om 16:19
-- Serverversie: 10.4.20-MariaDB
-- PHP-versie: 8.0.8

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `cryptomania`
--

-- --------------------------------------------------------

--
-- Tabelstructuur voor tabel `cryptofolio`
--

CREATE TABLE `cryptofolio` (
  `id` int(11) NOT NULL,
  `name` varchar(20) NOT NULL,
  `price` varchar(11) NOT NULL,
  `amount` int(11) NOT NULL,
  `totalValue` varchar(30) NOT NULL,
  `bought_on` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Gegevens worden geëxporteerd voor tabel `cryptofolio`
--

INSERT INTO `cryptofolio` (`id`, `name`, `price`, `amount`, `totalValue`, `bought_on`) VALUES
(25, 'Bitcoin', '7486.2', 1, '0', '2022-10-11'),
(26, 'Ripple', '0.650177', 33, '0', '2022-10-11');

--
-- Indexen voor geëxporteerde tabellen
--

--
-- Indexen voor tabel `cryptofolio`
--
ALTER TABLE `cryptofolio`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT voor geëxporteerde tabellen
--

--
-- AUTO_INCREMENT voor een tabel `cryptofolio`
--
ALTER TABLE `cryptofolio`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=27;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
