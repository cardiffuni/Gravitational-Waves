<?php 
    require 'sql-config.php';
    echo "<h1>Hello</h1>";

    try {
        $dbh = new PDO('mysql:host='. $hostname .';dbname='. $database, $username, $password);
        #echo "<p>connected</p>";
    } catch(PDOException $e) {
        echo '<h1>An error has ocurred.</h1><pre>', $e->getMessage() ,'</pre>';
    }
        
    #$hash = $_GET['hash']; 

    #$permitted_chars = '0123456789abcdefghijklmnopqrstuvwxyz';
    $permitted_chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';

    $gen_code = generate_string($permitted_chars, 6);

    #$gen_code = strtoupper (uniqid());
    #$gen_code = "HHHH";
    $name = $_GET['name'];
    $host_ip = $_GET['host_ip'];
    $team = $_GET['team'];
    echo "code: " . $gen_code;

    #$realHash = md5($_GET['name'] . $_GET['score'] . $secretKey); 
    #if($realHash == $hash) { 
    if(isset($name)){
        $sth = $dbh->prepare('INSERT INTO scores VALUES (null, :name, :gen_code, :host_ip, :team)');
        try {
            $worked = $sth->execute(array(':name' => $name, ':gen_code' => $gen_code, ':host_ip' => $host_ip, ':team' => $team));
            echo "<p> did it work? " . $worked . "</p>";
            echo "<p>returned</p>";
        } catch(Exception $e) {
            echo '<h1>An error has ocurred.</h1><pre>', $e->getMessage() ,'</pre>';
        }
    } else {
        echo "no data";
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