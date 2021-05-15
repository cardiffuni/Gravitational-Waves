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
    $team = $_GET['team'];

    #$realHash = md5($_GET['name'] . $_GET['score'] . $secretKey); 
    #if($realHash == $hash) { 
    if(isset($host_ip) && isset($team)){
        $sth = $dbh->prepare('INSERT INTO `scores`(`gen_code`, `host_ip`, `team`) VALUES (:gen_code, :host_ip, :team)');
        try {
            $gen_code = generate_string($permitted_chars, 6);
            echo "<p>trying code: <b>" . $gen_code . "</b></p>";

            $worked = $sth->execute(array(':gen_code' => $gen_code, ':host_ip' => $host_ip, ':team' => $team));

            echo "<p> did it work? <b>" . $worked . "</b></p>";

        } catch(Exception $e) {
            echo '<h1>An error has ocurred.</h1><pre>', $e->getMessage() ,'</pre>';
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
?>