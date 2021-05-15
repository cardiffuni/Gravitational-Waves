<?php 
    require 'sql-config.php';
    require 'sql-azure.php';
    require 'sql-connect.php';
    echo "<h1>Get Code</h1>";
    echo "<p>The time is " . date("Y-m-d h:i:sa") . "</p>";
        
    #$hash = $_GET['hash']; 

    #$permitted_chars = '0123456789abcdefghijklmnopqrstuvwxyz';
    $permitted_chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';

   
    $host_ip = $_GET['host_ip'];
    $teams = (int) filter_var($_GET['teams'], FILTER_SANITIZE_NUMBER_INT);
    #$realHash = md5($_GET['name'] . $_GET['score'] . $secretKey); 
    #if($realHash == $hash) { 
    if(isset($host_ip) && isset($teams) && $teams > 0){
        if($teams > 10){
            $teams = 10;
		}
        
        for ($team = 1; $team <= $teams; $team++) {
            for ($try = 1; $try <= 3; $try++) {
                echo "<p>team: <b>" . $team . "</b>, try: <b>" . $try . "</b></p>";
                $worked = insert_code($dbh, $host_ip, $team, $permitted_chars);
                echo "<p>Did it work? ";
                if ( isset($worked) && $worked > 0){
                    echo "<b>Yes</b>";
                    break;
			    } else {
                    echo "<b>No</b>";
			    }
                echo "</p>";
            }
        } 
        
    } else {
        echo "<p>invalid arguments</p>";
	}
    #} 

    function generate_string($input, $strength = 16) {
        $input_length = strlen($input);
        $random_string = '';
        for($i = 0; $i < $strength; $i++) {
            $random_character = $input[mt_rand(0, $input_length - 1)];
            $random_string .= $random_character;
        }
 
        return $random_string;
    }
    function insert_code($dbh, $host_ip, $team, $permitted_chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ') {
        $sth = $dbh->prepare('INSERT INTO `scores`(`gen_code`, `host_ip`, `team`) VALUES (:gen_code, :host_ip, :team)');
        try {
            
            $gen_code = generate_string($permitted_chars, 6);
            echo "<p>trying code: <b>" . $gen_code . "</b></p>";

            $worked = $sth->execute(array(':gen_code' => $gen_code, ':host_ip' => $host_ip, ':team' => $team));

            if (isset($worked) && $worked > 0){
                return true;
            } else {
                return false;
            }
        } catch(Exception $e) {
            echo '<h1>An error has ocurred.</h1><pre>', $e->getMessage() ,'</pre>';
            return false;
        }
    }
?>