#https://stats.stackexchange.com/questions/6759/removing-duplicated-rows-data-frame-in-r
rm(list = ls())

args = commandArgs(trailingOnly = TRUE) # allows R to get parameters


if (!require(mvtnorm)) install.packages("mvtnorm")
if (!require(ICSNP)) install.packages("ICSNP")
library(mvtnorm)
library(ICSNP)

p_size = as.numeric(args[1]) #obselete

trainingFilePathAndName <- args[2] #filepath and name
testingFilePathAndName <- args[3] #filepath and name

trainRem <- read.csv(trainingFilePathAndName, header = TRUE)
testRem <- read.csv(testingFilePathAndName, header = TRUE)

#removal of class column
trainRem <- trainRem[, 1:ncol(trainRem) - 1] #remove class col
testRem <- testRem[, 1:ncol(testRem) - 1] #remove class col

#remove duplicate instances
redtrainRem = trainRem[!duplicated(trainRem),]
redtestRem = testRem[!duplicated(testRem),]

maintrain <- vector(mode = "numeric", length = 0)
maintest <- vector(mode = "numeric", length = 0)

tempmaintrain <- vector(mode = "numeric", length = 0)
tempmaintest <- vector(mode = "numeric", length = 0)

for (i in 1:ncol(redtrainRem)) {
    tryCatch({
        tempmaintrain <- cbind(maintrain, redtrainRem[, c(i)])
        tempmaintest <- cbind(maintest, redtestRem[, c(i)])


        #two-sample multivariate t-test
        a = HotellingsT2(tempmaintrain, tempmaintest)

        #print(paste("add col",i, a$p.value))

        maintrain = tempmaintrain
        maintest = tempmaintest



    }, error = function(e) {

        #cat("ERROR :",conditionMessage(e), "\n")

    })



}

a$p.value






#cb <- function(df, sep="\t", dec=",", max.size=(200*1000)){
# Copy a data.frame to clipboard
#  write.table(df, paste0("clipboard-", formatC(max.size, format="f", digits=0)), sep=sep, row.names=FALSE, dec=dec)
#}

#cb(goodSet)
