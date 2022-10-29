pipeline {
  agent any
  stages {
    stage('Prune Docker data') {
      steps {
        sh 'docker system prune -a --volumes -f'
      }
    }
    stage('Start container') {
      steps {
        sh 'docker compose up -d  --wait'
        sh 'docker compose ps'
      }
    }
  }
}