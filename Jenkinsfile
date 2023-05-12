pipeline {
    agent any
    
    stages {
        stage("build") {
            setps {
                echo 'building the app'
                sh 'sudo apt-get install -y dotnet-sdk-6.0'
            }
        }

        stage("test") {
            steps {
                echo 'testing the app'
                sh 'cd GameStore/GameStoreTests | dotnet test'
            }
        }

        stage("deploy") {
            setps {
                echo 'deploying the app'
                sh 'cd ../GameStore | dotnet run'
            }
        }
    }
    post {
        always {
            echo 'Cleaning the workspace'
        }

        success {

        }

        failure {

        }
    }
}
