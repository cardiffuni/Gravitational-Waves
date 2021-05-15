<?php 
	// Parsing connnection string
	foreach ($_SERVER as $key => $value) {
		if (strpos($key, "MYSQLCONNSTR_") !== 0) {
			continue;
		}

		$hostname = preg_replace("/^.*Data Source=(.+?);.*$/", "\\1", $value);
		$database = preg_replace("/^.*Database=(.+?);.*$/", "\\1", $value);
		$username = preg_replace("/^.*User Id=(.+?);.*$/", "\\1", $value);
		$password = preg_replace("/^.*Password=(.+?)$/", "\\1", $value);
	}
?>