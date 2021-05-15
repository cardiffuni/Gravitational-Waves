<?php 

	try {
        $dbh = new PDO('mysql:host='. $hostname .';dbname='. $database, $username, $password);
        #echo "<p>connected</p>";
    } catch(PDOException $e) {
        echo '<h1>An error has ocurred.</h1><pre>', $e->getMessage() ,'</pre>';
    }
?>